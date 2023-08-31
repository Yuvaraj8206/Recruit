using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DisplayQuestion : MonoBehaviour
{
    public TMP_InputField examCodeInputField;
    public Text questionText;
    public TextMeshProUGUI sectionTitle1;
    public TextMeshProUGUI sectionTitle2;
    public TextMeshProUGUI ansQn;
    public TextMeshProUGUI correctAnscounttext;
    public Text[] optionTexts;
    public Button[] optionButtons;
    private int currentQuestionIndex = 0;
    private int correctansCount = 0;
    public GameObject previousButton;
    public GameObject nextButton;
    public GameObject nextsectionButton;
    public GameObject previoussectionButton;
    private int selectedOptionIndex = -1;
    public Button startButton;
    private int i = 1;
    private bool[] optionSelected;

    private List<Dictionary<string, object>> hardcodedQuestions = new List<Dictionary<string, object>>
    {
        new Dictionary<string, object>
        {
            {"objid", 1 },
            {"question", "Choose the correct synonym for exquisite"},
            {"options", new List<string>{ "Beautiful", "Ordinary", "Ugly", "Clumsy"}},
            {"correctOption", "Beautiful"},
            {"question_type", "verbal" },
            {"selectedOption", ""}
        },
        new Dictionary<string, object>
        {
            {"objid", 2 },
            {"question", "Which word is the opposite of abundant?"},
            {"options", new List<string>{ "Scarce", "Ample", "Plentiful", "Copious"}},
            {"correctOption", "Scarce"},
            {"question_type", "verbal" },
            {"selectedOption", ""}
        },
        new Dictionary<string, object>
        {
            {"objid", 3 },
            {"question", "Select the word that best fits the given definition: A person who studies fossils."},
            {"options", new List<string>{ "Paleontologist", "Astronomer", "Botanist", "Geologist"}},
            {"correctOption", "Paleontologist"},
            {"question_type", "verbal" },
            {"selectedOption", ""}
        },
        new Dictionary<string, object>
        {
            {"objid", 4 },
            {"question", "Identify the correct spelling:"},
            {"options", new List<string>{ "Occurrence", "Occurence", "Occurrance", "Occurance"}},
            {"correctOption", "Occurrence"},
            {"question_type", "verbal" },
            {"selectedOption", ""}
        },
        new Dictionary<string, object>
        {
            {"objid", 5 },
            {"question", "Choose the correct antonym for captivate:"},
            {"options", new List<string>{ "Bore", "Engross", "Fascinate", "Entrance"}},
            {"correctOption", "Bore"},
            {"question_type", "verbal" },
            {"selectedOption", ""}
        },
        new Dictionary<string, object>
        {
            {"objid", 6 },
            {"question", "Which of the following is a programming language?"},
            {"options", new List<string>{ "HTML", "CSS", "JavaScript", "XML"}},
            {"correctOption", "HTML"},
            {"question_type", "programming" },
            {"selectedOption", ""}
        },
        new Dictionary<string, object>
        {
            {"objid", 7 },
            {"question", "What is the output of the following code snippet?\n\nint x = 5;\nint y = x++ + ++x;\nprint(y);"},
            {"options", new List<string>{ "10", "11", "12", "14"}},
            {"correctOption", "14"},
            {"question_type", "programming" },
            {"selectedOption", ""}
        },
        new Dictionary<string, object>
        {
            {"objid", 8 },
            {"question", "Which statement is used to terminate a loop prematurely?"},
            {"options", new List<string>{ "break", "continue", "return", "exit"}},
            {"correctOption", "break"},
            {"question_type", "programming" },
            {"selectedOption", ""}
        },
        new Dictionary<string, object>
        {
            {"objid", 9 },
            {"question", "What is the correct syntax to declare an array in Python?"},
            {"options", new List<string>{ "array A;", "int[] A;", "A = Array();", "A = [];"}},
            {"correctOption", "A = [];"},
            {"question_type", "programming" },
            {"selectedOption", ""}
        },
        new Dictionary<string, object>
        {
            {"objid", 10 },
            {"question", "In object-oriented programming, the process of creating an instance of a class is called:"},
            {"options", new List<string>{ "Inheritance", "Polymorphism", "Encapsulation", "Instantiation"}},
            {"correctOption", "Instantiation"},
            {"question_type", "programming" },
            {"selectedOption", ""}
        },
    };

    private void Start()
    {
        /*LoadQuestions(currentQuestionIndex, null);
        SetPreviousButtonVisibility(false);*/
        optionSelected = new bool[optionButtons.Length];
        LoadQuestions(currentQuestionIndex, null);
        SetPreviousButtonVisibility(false);
        examCodeInputField.onValueChanged.AddListener(OnExamCodeValueChanged);
        startButton.interactable = false;
    }
    private void OnExamCodeValueChanged(string value)
    {
        // Check if the entered value matches the hard-coded examCodeInputField
        if (value == "444")
        {
            startButton.interactable = true; // Enable the start button
        }
        else
        {
            startButton.interactable = false; // Disable the start button
        }
    }
    private void LoadQuestions(int questionIndex, string questionType)
    {
        List<Dictionary<string, object>> filteredQuestions;

        if (string.IsNullOrEmpty(questionType))
        {
            filteredQuestions = hardcodedQuestions;
        }
        else
        {
            filteredQuestions = hardcodedQuestions.FindAll(q => (string)q["question_type"] == questionType);
        }

        if (questionIndex >= 0 && questionIndex < filteredQuestions.Count)
        {
            Dictionary<string, object> question = filteredQuestions[questionIndex];
            string questionString = (string)question["question"];
            List<string> options = (List<string>)question["options"];
            string correctOption = (string)question["correctOption"];
            string selectedOption = (string)question["selectedOption"];

            // Assign question text
            questionText.text = questionString;

            // Assign options text and highlight the selected option
            for (int i = 0; i < optionTexts.Length; i++)
            {
                optionTexts[i].text = options[i];

                // Highlight the selected option
                if (options[i] == selectedOption)
                {
                    optionButtons[i].interactable = false;
                    optionSelected[i] = true;
                    optionButtons[i].image.color = HexToColor("#0079FF");
                }
                else
                {
                    optionButtons[i].interactable = true;
                    optionSelected[i] = false;
                    optionButtons[i].image.color = Color.white;
                }
                
                // Add OnClick event to each option button
                int index = i; // Store the index in a separate variable to avoid closure issues
                optionButtons[i].onClick.RemoveAllListeners(); // Remove any previous listeners
                optionButtons[i].onClick.AddListener(() => OnOptionClicked(index, correctOption));
            }

            // Assign section title based on question type
            string sectionTitle = (string)question["question_type"] == "verbal" ? "Verbal Section" : "Programming Section";
            sectionTitle1.text = sectionTitle;

            SetPreviousButtonVisibility(questionIndex > 0);
        }
    }
    private void OnOptionClicked(int selectedIndex, string correctOption)
    {
        // Enable all options
        for (int i = 0; i < optionSelected.Length; i++)
        {
            optionButtons[i].interactable = true;
            optionSelected[i] = false;
            optionButtons[i].image.color = Color.white;
        }

        // Highlight the selected option
        optionButtons[selectedIndex].image.color = HexToColor("#0079FF");
        optionSelected[selectedIndex] = true;

        selectedOptionIndex = selectedIndex;

        Dictionary<string, object> question = hardcodedQuestions[currentQuestionIndex];
        string selectedOption = optionTexts[selectedIndex].text;

        question["selectedOption"] = selectedOption;

        if (selectedOption == correctOption)
        {
            Debug.Log("Correct");
            correctansCount++;
        }
        else
        {
            Debug.Log("Wrong");
        }
    }

    public void LoadVerbalQuestions()
    {
        currentQuestionIndex = 0;
        LoadQuestions(currentQuestionIndex, "verbal");
        correctansCount = 0;
        UpdateCorrectAnswerCountText();
    }

    public void LoadProgrammingQuestions()
    {
        currentQuestionIndex = 0;
        LoadQuestions(currentQuestionIndex, "programming");
        correctansCount = 0;
        UpdateCorrectAnswerCountText();
    }

    public void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex < hardcodedQuestions.Count)
        {
            LoadQuestions(currentQuestionIndex, null);
            SetPreviousButtonVisibility(true);
            i++;
            ansQn.text = i.ToString();
            if (currentQuestionIndex == hardcodedQuestions.Count - 1)
            {
                SetNextButtonVisibility(false);
                SetNextSectionButtonVisibility(true);
            }
        }
        else
        {
            Debug.Log("Display next question");
            SetNextButtonVisibility(false);
            SetNextSectionButtonVisibility(true);
        }
    }
    public void PreviousQuestion()
    {
        currentQuestionIndex--;
        if (currentQuestionIndex >= 0)
        {
            LoadQuestions(currentQuestionIndex, null);
            --i;
            ansQn.text = i.ToString();
            SetNextButtonVisibility(true);
            SetNextSectionButtonVisibility(false);
        }
        else
        {
            Debug.Log("Display previous question");
            SetPreviousButtonVisibility(false);
        }
    }

    private void SetPreviousButtonVisibility(bool isVisible)
    {
        if (previousButton != null)
        {
            previousButton.SetActive(isVisible);
        }
    }
    private void SetNextButtonVisibility(bool isVisible)
    {
        if (nextButton != null)
        {
            nextButton.SetActive(isVisible);
        }
    }
    private void SetNextSectionButtonVisibility(bool isVisible)
    {
        if (nextsectionButton != null)
        {
            nextsectionButton.SetActive(isVisible);
        }
    }
    public void ShowResult()
    {
        Debug.Log("Total Correct Answers: " + correctansCount);
        UpdateCorrectAnswerCountText();
    }
    private void UpdateCorrectAnswerCountText()
    {
        if (correctAnscounttext != null)
        {
            correctAnscounttext.text = correctansCount.ToString();
        }
    }
    private Color HexToColor(string hex)
    {
        Color color = Color.white;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else
        {
            Debug.LogWarning("Invalid color hex code: " + hex);
            return Color.white;
        }
    }

}