using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using TMPro;

public class FetchQuestions : MonoBehaviour
{
    public TMP_InputField examCodeInputField;
    public Text questionText;
    public Text[] optionTexts;
    public Button startButton;
    public Button previousButton;
    private MySqlConnection connection;
    private MySqlCommand command;
    private string connectionString = "Server=localhost; port=3306; Database=prodaptrecruitmentplan; user=root; password=root;";
    private Dictionary<string, List<string>> verbalQuestions = new Dictionary<string, List<string>>();
    private Dictionary<string, List<string>> programmingQuestions = new Dictionary<string, List<string>>();
    private List<string> verbalQuestionCodes = new List<string>();
    private List<string> programmingQuestionCodes = new List<string>();
    private int currentQuestionIndex = 0;
    private string currentQuestionType = "";
    private bool isValidCode = false;

    private void Start()
    {
        // Code for Start method remains unchanged...
    }

    public void StartExam()
    {
        string examCode = examCodeInputField.text;

        if (!string.IsNullOrEmpty(examCode))
        {
            ConnectToDatabase();

            if (connection.State == ConnectionState.Open)
            {
                isValidCode = CheckValidExamCode(examCode);

                if (isValidCode)
                {
                    FetchQuestionsAndOptionsFromDatabase(examCode);
                    currentQuestionIndex = 0;

                    // Determine the current question type based on the first question
                    currentQuestionType = GetCurrentQuestionType();

                    // Show the first question
                    ShowCurrentQuestion();

                    // Disable the "Previous" button when showing the first question
                    previousButton.interactable = false;
                }
                else
                {
                    Debug.Log("Invalid exam code. Please enter a valid code.");
                }
            }
        }
        else
        {
            Debug.Log("Please enter an exam code.");
        }
    }

    public void NextQuestion()
    {
        List<string> questionCodesList = GetCurrentQuestionCodesList();

        if (currentQuestionType == "verbal")
        {
            // Check if we have reached the last verbal question
            if (currentQuestionIndex < questionCodesList.Count - 1)
            {
                currentQuestionIndex++;
            }
            else
            {
                // If we have reached the last verbal question, switch to programming questions
                currentQuestionType = "programming";
                currentQuestionIndex = 0;
            }
        }
        else if (currentQuestionType == "programming")
        {
            // Check if we have reached the last programming question
            if (currentQuestionIndex < questionCodesList.Count - 1)
            {
                currentQuestionIndex++;
            }
            else
            {
                // If we have reached the last programming question, do nothing or handle as needed.
                Debug.Log("This is the last question.");
                return;
            }
        }

        ShowCurrentQuestion();
        previousButton.interactable = currentQuestionIndex > 0;
    }

    public void PreviousQuestion()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            currentQuestionType = GetCurrentQuestionType();
            ShowCurrentQuestion();
            previousButton.interactable = currentQuestionIndex > 0;
        }
        else
        {
            Debug.Log("This is the first question.");
        }
    }

    private void ShowCurrentQuestion()
    {
        if (currentQuestionType == "verbal")
        {
            ShowQuestionAndOptions(currentQuestionIndex);
        }
        else if (currentQuestionType == "programming")
        {
            // Ensure the programmingQuestions dictionary and programmingQuestionCodes list are not empty
            if (programmingQuestions.Count > 0 && programmingQuestionCodes.Count > 0)
            {
                ShowProgrammingQuestionAndOptions(currentQuestionIndex);
            }
            else
            {
                Debug.LogError("No programming questions found.");
            }
        }
    }

    private void ConnectToDatabase()
    {
        connection = new MySqlConnection(connectionString);
        command = connection.CreateCommand();

        try
        {
            connection.Open();
            Debug.Log("Connected to the database.");
        }
        catch (MySqlException ex)
        {
            Debug.LogError("Failed to connect to the database: " + ex.Message);
        }
    }

    private bool CheckValidExamCode(string examCode)
    {
        string query = "SELECT code FROM table_hr_mail_codes WHERE code = @examCode";
        command.CommandText = query;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@examCode", examCode);

        using (MySqlDataReader reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return true; // The exam code is valid
            }
        }

        return false; // The exam code is not valid
    }

    private void FetchQuestionsAndOptionsFromDatabase(string examCode)
    {
        string query = "SELECT question_text, option1, option2, option3, option4, question_type FROM table_questions " +
            "INNER JOIN table_hr_mail_codes ON table_questions.question2code = table_hr_mail_codes.objid " +
            "WHERE table_hr_mail_codes.code = @examCode";

        command.CommandText = query;
        command.Parameters.Clear(); // Clear any existing parameters

        // Add the examCode parameter
        command.Parameters.AddWithValue("@examCode", examCode);

        verbalQuestions.Clear();
        programmingQuestions.Clear();
        verbalQuestionCodes.Clear();
        programmingQuestionCodes.Clear();

        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string questionText = reader.GetString("question_text");
                string option1 = reader.GetString("option1");
                string option2 = reader.GetString("option2");
                string option3 = reader.GetString("option3");
                string option4 = reader.GetString("option4");
                string questionType = reader.GetString("question_type");

                List<string> questionData = new List<string> { questionText, option1, option2, option3, option4 };

                if (questionType == "verbal")
                {
                    verbalQuestions.Add(questionText, questionData);
                    verbalQuestionCodes.Add(questionText);
                }
                else if (questionType == "programming")
                {
                    programmingQuestions.Add(questionText, questionData);
                    programmingQuestionCodes.Add(questionText);
                }
            }
        }
    }

    private List<string> GetCurrentQuestionCodesList()
    {
        // Get the current question codes list based on the question type
        string currentQuestionType = GetCurrentQuestionType();
        return currentQuestionType == "verbal" ? verbalQuestionCodes : programmingQuestionCodes;
    }

    private string GetCurrentQuestionType()
    {
        // Get the current question type based on the current question index
        if (verbalQuestionCodes.Count > 0 && currentQuestionIndex >= 0 && currentQuestionIndex < verbalQuestionCodes.Count)
        {
            return "verbal";
        }
        else if (programmingQuestionCodes.Count > 0 && currentQuestionIndex >= 0 && currentQuestionIndex < programmingQuestionCodes.Count)
        {
            return "programming";
        }
        return "";
    }

    private void ShowQuestionAndOptions(int questionIndex)
    {
        List<string> questionCodesList = GetCurrentQuestionCodesList();

        if (questionCodesList.Count > 0 && questionIndex >= 0 && questionIndex < questionCodesList.Count)
        {
            string questionCode = questionCodesList[questionIndex];
            Dictionary<string, List<string>> currentQuestions = GetCurrentQuestionDictionary();

            if (currentQuestions.TryGetValue(questionCode, out List<string> questionData))
            {
                // Update the question text
                questionText.text = questionData[0];

                // Update the options text
                for (int j = 0; j < optionTexts.Length; j++)
                {
                    if (j < questionData.Count - 1)
                    {
                        optionTexts[j].text = questionData[j + 1];
                    }
                }
            }
            else
            {
                // Display an error message if the question code is not found in the current questions dictionary
                Debug.LogError("Question code '" + questionCode + "' not found in the current questions dictionary.");
            }
        }
    }

    private void ShowProgrammingQuestionAndOptions(int questionIndex)
    {
        List<string> questionCodesList = GetCurrentQuestionCodesList();

        if (questionCodesList.Count > 0 && questionIndex >= 0 && questionIndex < questionCodesList.Count)
        {
            string questionCode = questionCodesList[questionIndex];
            Dictionary<string, List<string>> currentQuestions = GetCurrentQuestionDictionary();

            if (currentQuestions.TryGetValue(questionCode, out List<string> questionData))
            {
                // Update the question text
                questionText.text = questionData[0];

                // Update the options text (assuming programming questions have only one option)
                if (optionTexts.Length > 0)
                {
                    optionTexts[0].text = "Write your answer here...";
                }
            }
            else
            {
                // Display an error message if the question code is not found in the current questions dictionary
                Debug.LogError("Question code '" + questionCode + "' not found in the current questions dictionary.");
            }
        }
    }

    private Dictionary<string, List<string>> GetCurrentQuestionDictionary()
    {
        // Get the current question dictionary based on the question type
        string currentQuestionType = GetCurrentQuestionType();
        return currentQuestionType == "verbal" ? verbalQuestions : programmingQuestions;
    }

    private void OnDestroy()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
            Debug.Log("Disconnected from the database.");
        }
    }
}
