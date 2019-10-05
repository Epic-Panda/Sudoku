using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool eog;

    public GameObject EOGPanel;
    public Text eogTimeText;

    public Text timeTxt;
    public Text[] numbers;
    bool backTrack = false;
    int[] numbersInt = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    List<List<Num>> lista;

    int fieldsToFill;
    int selectedNumber;
    bool deleteNumber;
    bool checkAnswer;
    int levelDifficulty;

    float timeNum;

    List<List<bool>> locked;

    // number button
    Button oldButton;

    MenuManager menuManager;

    struct Num
    {
        public int box;

        public List<int> correctAnswer;

        public List<int> notTriedNumbers;

        public Text number;

        public Num(Text n)
        {
            correctAnswer = new List<int>();
            box = -1;
            notTriedNumbers = new List<int>();
            number = n;
        }
    }


    void Start()
    {
        menuManager = GameObject.Find("MenuController").GetComponent<MenuManager>();
        levelDifficulty = menuManager.levelDifficulty;

        eog = false;
        timeNum = 0;
        fieldsToFill = 81;
        selectedNumber = 1;
        deleteNumber = false;
        checkAnswer = false;

        PrepareList();

        // needed for random numbers
        Random.seed = System.DateTime.Now.Millisecond;

        CreateNumbers();

        ShowNumbers();
    }

    void Update()
    {
        if (eog)
            return;

        timeNum += Time.deltaTime;
        int sec = Mathf.FloorToInt(timeNum % 60);
        if (sec < 10)
            timeTxt.text = Mathf.FloorToInt(timeNum / 60) + ":0" + sec;
        else
            timeTxt.text = Mathf.FloorToInt(timeNum / 60) + ":" + sec;
    }

    void CreateNumbers()
    {
        for (int i = 0, j = 0; j < lista.Count; i++)
        {
            Recursive(i, j);

            // if need to backtrack return two spaces back, loop will go to next one
            if (backTrack)
            {
                lista[j][i].correctAnswer.Clear();
                lista[j][i].notTriedNumbers.Clear();
                lista[j][i].notTriedNumbers.AddRange(numbersInt);
                lista[j][i].number.text = "";

                if (i == 0)
                {
                    i = 6;
                    j--;
                }
                else if (i == 1)
                {
                    i = 7;
                    j--;
                }
                else
                    i -= 2;

                backTrack = false;
            }

            if (i == 8)
            {
                i = -1;
                j++;
            }
        }
    }

    void Recursive(int i, int j)
    {
        int box = j;

        if (lista[j][i].notTriedNumbers.Count == 0)
        {
            backTrack = true;
            return;
        }

        int randNum = lista[j][i].notTriedNumbers[Random.Range(0, lista[j][i].notTriedNumbers.Count)];

        lista[j][i].notTriedNumbers.Remove(randNum);

        // check for small grid (box)
        for (int l = 0; l < lista.Count; l++)
        {
            for (int m = 0; m < lista[l].Count; m++)
                // changed
                if (lista[l][m].box == lista[j][i].box /*box*/)
                {
                    if (lista[l][m].correctAnswer.Count > 0)
                        if (randNum == lista[l][m].correctAnswer[0])
                        {
                            Recursive(i, j);
                            return;
                        }
                }
        }

        // check row
        for (int n = 0; n < lista[j].Count; n++) if (lista[j][n].correctAnswer.Count > 0)
                if (randNum == lista[j][n].correctAnswer[0])
                {
                    Recursive(i, j);
                    return;
                }

        // check for column
        for (int n = 0; n < lista.Count; n++) if (lista[n][i].correctAnswer.Count > 0)
                if (randNum == lista[n][i].correctAnswer[0])
                {
                    Recursive(i, j);
                    return;
                }

        lista[j][i].correctAnswer.Add(randNum);
    }

    void ShowNumbers()
    {
        List<int> avNumber = new List<int>();
        avNumber.AddRange(numbersInt);

        int min = 3, max = 5;

        // easy
        if (levelDifficulty == 0)
        {
            min = 3;
            max = 5;

        }
        else // hard
        {
            min = 1;
            max = 4;
        }


        for (int i = 0; i < 9; i++)
        {
            avNumber.Clear();
            avNumber.AddRange(numbersInt);

            int numOfNumbers = Random.Range(min, max);

            for (int j = 0; j < numOfNumbers; j++)
            {
                int col = avNumber[Random.Range(0, avNumber.Count)];
                avNumber.Remove(col);
                col--;

                // lock field with number
                locked[i][col] = true;

                // add number
                lista[i][col].number.text = lista[i][col].correctAnswer[0].ToString();
                lista[i][col].number.GetComponentInChildren<Image>().color = new Color(72f / 255, 72f / 255, 72f / 255, 98f / 255);
                fieldsToFill--;
            }
        }
    }

    void PrepareList()
    {
        lista = new List<List<Num>>(9);
        lista.Add(new List<Num>());

        locked = new List<List<bool>>();
        locked.Add(new List<bool>());

        for (int i = 0, j = 0, k = 0; i < numbers.Length; i++, k++)
        {
            if (k == 9)
            {
                k = 0;
                j++;
                lista.Add(new List<Num>());
                locked.Add(new List<bool>());
            }

            Num n = new Num(numbers[i]);

            if (j < 3)
            {
                if (k < 3)
                    n.box = 1;
                else if (k < 6)
                    n.box = 2;
                else
                    n.box = 3;
            }
            else if (j < 6)
            {
                if (k < 3)
                    n.box = 4;
                else if (k < 6)
                    n.box = 5;
                else
                    n.box = 6;
            }
            else
            {
                if (k < 3)
                    n.box = 7;
                else if (k < 6)
                    n.box = 8;
                else
                    n.box = 9;
            }

            n.notTriedNumbers.AddRange(numbersInt);
            lista[j].Add(n);

            locked[j].Add(false);
        }
    }

    void EndOfGame()
    {
        eog = true;
        EOGPanel.active = true;
        eogTimeText.text = "Time: " + timeTxt.text;
    }

    public void SetSelectedNumber(int number)
    {
        selectedNumber = number;
    }

    public void SetSelectedButton(Button btn)
    {
        if (oldButton)
            oldButton.GetComponent<Image>().color = new Color(72f / 255, 72f / 255, 72f / 255, 98f / 255);

        oldButton = btn;
        oldButton.GetComponent<Image>().color = new Color(0, 1, 0);
    }

    public void DeleteButton(Button btn)
    {
        if (deleteNumber)
        {
            btn.GetComponent<Image>().color = new Color(1, 1, 1);
            deleteNumber = false;
            return;
        }

        deleteNumber = true;
        btn.GetComponent<Image>().color = new Color(1, 0, 0);
    }

    public void SelectField(Text t)
    {
        if (deleteNumber)
        {
            if (t.text != "")
                fieldsToFill++;

            t.text = "";

            if (checkAnswer)
                t.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0);

            return;
        }

        int col = 0, row = 0;
        for (int i = 0; i < lista.Count; i++)
            for (int j = 0; j < lista[i].Count; j++)
                if (t == lista[i][j].number)
                {
                    col = j;
                    row = i;
                    break;
                }

        for (int i = 0; i < lista.Count; i++)
        {
            // check if is in column
            if (lista[i][col].number.text == selectedNumber.ToString())
                return;

            for (int j = 0; j < lista[i].Count; j++)
            {
                // check if number is in same box
                if (lista[i][j].box == lista[row][col].box && lista[i][j].number.text == selectedNumber.ToString())
                    return;

                // check row
                if (i == 0) // check only once
                    if (lista[row][j].number.text == selectedNumber.ToString())
                        return;
            }
        }

        if (lista[row][col].number.text == "")
            fieldsToFill--;

        lista[row][col].number.text = selectedNumber.ToString();

        if (checkAnswer)
        {
            if (lista[row][col].correctAnswer[0] == selectedNumber)
                lista[row][col].number.GetComponentInChildren<Image>().color = new Color(0, 1, 0, 128f / 255);
            else
                lista[row][col].number.GetComponentInChildren<Image>().color = new Color(1, 0, 0, 128f / 255);
        }

        if (fieldsToFill <= 0)
            EndOfGame();
    }

    public void CheckIfIsCorect(Button btn)
    {
        if (!checkAnswer)
        {
            btn.GetComponent<Image>().color = new Color(0, 1, 0);
            checkAnswer = true;

            for (int i = 0; i < lista.Count; i++)
                for (int j = 0; j < lista[i].Count; j++)
                    // if field is not empty or locked check if it is correct
                    if (lista[i][j].number.text != "" && !locked[i][j])
                        if (lista[i][j].number.text == lista[i][j].correctAnswer[0].ToString())
                            lista[i][j].number.GetComponentInChildren<Image>().color = new Color(0, 1, 0, 128f / 255);
                        else
                            lista[i][j].number.GetComponentInChildren<Image>().color = new Color(1, 0, 0, 128f / 255);
        }
        else
        {
            btn.GetComponent<Image>().color = new Color(1, 1, 1);
            checkAnswer = false;

            for (int i = 0; i < lista.Count; i++)
                for (int j = 0; j < lista[i].Count; j++)
                    if (!locked[i][j])
                        lista[i][j].number.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0);
        }
    }

    public void Menu()
    {
        eog = true;
        Destroy(menuManager.gameObject);
        SceneManager.LoadScene("Menu");
    }

    public void PlayAgain()
    {
        eog = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
