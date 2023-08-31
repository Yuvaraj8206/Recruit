/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;



public class QuestionManager : MonoBehaviour
{
    public Text questionText;
    public Button[] optionButtons;
    public Button nextButton;
    public Button prevButton;
    public InputField examCodeInput;
    public Button startButton;
    public Text[] optionTexts;
    public Color normalColor;
    public Color selectedColor;



    private List<Dictionary<string, string>> questions;
    private int currentQuestionIndex = 0;



    private MySqlConnection dbConnection;
    private string connectionString = "Server=localhost; port=3306; Database=prodaptrecruitmentplan; user=root; password=root;"; // Replace this with your database connection string



    private int userId = 1; // Replace this with the specific user's ID (You'll need to implement this based on your user system)



    private void Start()
    {
        // Initialize database connection
        dbConnection = new MySqlConnection(connectionString);



        // Disable the next and previous buttons until the exam starts
        nextButton.interactable = false;
        prevButton.interactable = false;



        // Add an onClick listener to the Start button
        startButton.onClick.AddListener(StartExam);
    }



    private void OnDestroy()
    {
        if (dbConnection != null && dbConnection.State == System.Data.ConnectionState.Open)
        {
            dbConnection.Close();
        }
    }



    private void StartExam()
    {
        // Clear the questions list and reset the current question index
        questions = new List<Dictionary<string, string>>();
        currentQuestionIndex = 0;



        // Parse the exam code from the InputField
        int examCode;
        if (!int.TryParse(examCodeInput.text, out examCode))
        {
            Debug.LogError("Invalid exam code.");
            return;
        }



        // Load questions and options from the database based on the exam code provided by the user
        LoadQuestionsFromDatabase(examCode);



        // If no questions are found for the given exam code, show an error message and return
        if (questions.Count == 0)
        {
            Debug.LogError("No questions found for the provided exam code.");
            return;
        }



        // Display the first question
        ShowQuestion(currentQuestionIndex);



        // Enable the next and previous buttons after the exam starts
        nextButton.interactable = true;
        prevButton.interactable = true;



        // Debug log the user mapped with the corresponding code
        string userEmail = GetUserEmailForExamCode(examCode);
        if (string.IsNullOrEmpty(userEmail))
        {
            Debug.Log("No user found for the provided exam code.");
        }
        else
        {
            Debug.Log("User Email: " + userEmail + ", Exam Code: " + examCode);
        }
    }



    private void LoadQuestionsFromDatabase(int examCode)
    {
        // Load questions and options from the database based on the provided exam code
        try
        {
            dbConnection.Open();



            string query = "SELECT objid, question_text, option1, option2, option3, option4 " +
                           "FROM table_questions WHERE question2code = @examCode";



            MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@examCode", examCode);
            MySqlDataReader reader = cmd.ExecuteReader();


            List<Dictionary<string, string>> questions = new List<Dictionary<string, string>>();

            while (reader.Read())
            {
                
                *//*questionData["objid"] = reader.GetInt64(0).ToString();
                questionData["question_text"] = reader.GetString(1);
                questionData["option1"] = reader.GetString(2);
                questionData["option2"] = reader.GetString(3);
                questionData["option3"] = reader.GetString(4);
                questionData["option4"] = reader.GetString(5);
                questionData["correct_option"] = reader.GetString(6);*//*
                string question = reader.GetString("question_text");
                string option1 = reader.GetString("option1");
                string option2 = reader.GetString("option2");
                string option3 = reader.GetString("option3");
                string option4 = reader.GetString("option4");

                Dictionary<string, string> questionData = new Dictionary<string, string>
                {
                    question,
                    option1,
                    option2,
                    option3,
                    option4
                };
                questions.Add(questionData);
            }
 
            reader.Close();
        }

        catch (Exception e)
        {
            Debug.LogError("Error loading questions from the database: " + e.Message);
        }
        finally
        {
            dbConnection.Close();
        }
    }

    private void ShowQuestion(int questionIndex)
    {
        if (questions.Count == 0 || questionIndex < 0 || questionIndex >= questions.Count)
        {
            return;
        }
        Dictionary<string, string> questionData = questions[questionIndex];
        questionText.text = questionData["question_text"];



        for (int i = 0; i < optionButtons.Length; i++)
        {
            string optionKey = "option" + (i + 1);
            optionButtons[i].gameObject.SetActive(true);
            optionButtons[i].GetComponentInChildren<Text>().text = questionData[optionKey];
            optionButtons[i].interactable = true;
            optionButtons[i].GetComponent<Image>().color = normalColor;
        }
    }


    public void SelectOption(int optionIndex)
    {
        if (currentQuestionIndex < 0 || currentQuestionIndex >= questions.Count)
        {
            return;
        }



        string selectedOption = optionButtons[optionIndex].GetComponentInChildren<Text>().text;
        int questionId = int.Parse(questions[currentQuestionIndex]["objid"]);



        // Save the selected option in the table_answer table
        SaveSelectedOption(questionId, selectedOption);



        // Check if the selected option matches the correct option to increment the score
        if (selectedOption == questions[currentQuestionIndex]["correct_option"])
        {
            //IncrementScore();
            Debug.Log("Correct option");
        }



        // Highlight the selected option button
        foreach (Button optionButton in optionButtons)
        {
            optionButton.interactable = false;
        }
        optionButtons[optionIndex].GetComponent<Image>().color = selectedColor;
    }



    private void SaveSelectedOption(int questionId, string selectedOption)
    {
        try
        {
            dbConnection.Open();

            string query = "INSERT INTO table_answer (selected_option, answer2question, answer2code, answer2user) " +
                           "VALUES (@selectedOption, @questionId, @examCode, @userId)";

            MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@selectedOption", selectedOption);
            cmd.Parameters.AddWithValue("@questionId", questionId);
            cmd.Parameters.AddWithValue("@examCode", examCodeInput.text);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving selected option to the database: " + e.Message);
        }
        finally
        {
            dbConnection.Close();
        }
    }
    private string GetUserEmailForExamCode(int examCode)
    {
        string userEmail = null;
        try
        {
            dbConnection.Open();

            string query = "SELECT email FROM table_hr_mail_codes WHERE code = @examCode";
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@examCode", examCode);

            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                userEmail = result.ToString();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error getting user email from the database: " + e.Message);
        }
        finally
        {
            dbConnection.Close();
        }

        return userEmail;
    }
}*/