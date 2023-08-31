using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    public int questionIndex;
    public int optionIndex;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    public void HandleClick()
    {
        FetchQuestions fetchQuestions = FindObjectOfType<FetchQuestions>();
        if (fetchQuestions != null)
        {
            //fetchQuestions.OnOptionSelected(questionIndex, optionIndex);
        }
    }
}
