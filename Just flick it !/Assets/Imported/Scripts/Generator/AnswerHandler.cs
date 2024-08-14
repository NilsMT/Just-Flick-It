using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AnswerHandler : MonoBehaviour
{
    [Header("Answer Visualization")]
    public bool AnswerVisualization=false;
    public GameCycler gamecycler;

    public bool IsProblemSolved(string[,] answer, string[,] current)
    {
        bool result = true;
        for (int i = 0; i < current.GetLength(0); i++)
        {
            for (int j = 0; j < current.GetLength(1); j++)
            {
                if (current[i,j] != answer[i, j])
                {
                    result = false;
                }
            }
        }
        return result;
    }
    char OppositeStatus(char status)
    {
        char r = '0'; if (status == '0') {r = '1'; } return r;
    }
    //Solving algorithms

    /*if (RealtimeModification)
    {
        GameObject switchmodel = transform.GetComponent<GameGenerator>().GetObjectByCoords(i, j);
        switchmodel.GetComponent<Outline>().OutlineColor = Color.red;
    }*/
    void SolveLine(string[,] answer)
    {
        for (int j = 0; j < answer.GetLength(1); j++)
        {
            //Get the majority
            int CountOne = 0;
            int CountZero = 0;
            for (int i = 0; i < answer.GetLength(0); i++)
            {
                if (answer[i,j]!="")
                {
                    char status = answer[i,j][1];
                    if (answer[i,j][0] == 'L')
                    {
                        if (status == '0')
                        {
                            CountZero++;
                        }
                        else if (status == '1')
                        {
                            CountOne++;
                        }
                    }
                }
            }
            //Set each Line value
            for (int i = 0; i < answer.GetLength(0); i++)
            {
                if (answer[i,j] != "")
                {
                    char status = answer[i,j][1];
                    if (answer[i,j][0] == 'L')
                    {
                        if (CountZero < CountOne) //all to one
                        {
                            status = '1' ;
                        }
                        else if (CountZero > CountOne)  //all to zero
                        {
                            status = '0';
                        } else if (CountZero == CountOne) //equal
                        {
                            status = OppositeStatus(status);
                        }
                        answer[i,j] = answer[i,j][0].ToString() + status;
                    }
                }
            }
            
        }
    }
    void SolveTriangle(string[,] answer)
    {
        for (int i = 0; i < answer.GetLength(0); i++)
        {
            for (int j = 0; j < answer.GetLength(1); j++)
            {
                if (answer[i, j] != "")
                {
                    char status = answer[i, j][1];
                    if (answer[i, j][0] == 'T')
                    {
                        try
                        {
                            string left = answer[(i), (j - 1)];
                            string right = answer[(i), (j + 1)];
                            string top = answer[(i - 1), (j)];

                            char topstatus = top[1];

                            if (answer[(i), (j - 1)]!="")
                            {
                                answer[(i), (j - 1)] = left[0].ToString() + OppositeStatus(topstatus);
                            }

                            if (answer[(i), (j + 1)] != "")
                            {
                                answer[(i), (j + 1)] = right[0].ToString() + OppositeStatus(topstatus);
                            }
                            
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            status=OppositeStatus(status);
                            answer[i, j] = "T" + status;
                        }
                    }
                }
            }
        }
    }
    void SolveSquare(string[,] answer)
    {
        for (int i = 0; i < answer.GetLength(0); i++)
        {
            for (int j = 0; j < answer.GetLength(1); j++)
            {
                if (answer[i, j] != "")
                {
                    char status = answer[i, j][1];
                    if (answer[i, j][0] == 'S')
                    {
                        //Doing A
                        try
                        {
                            if (answer[(i - 1), (j - 1)]!="")
                            {
                                answer[(i - 1), (j - 1)] = answer[(i - 1), (j - 1)][0].ToString() + OppositeStatus(status);
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                        try
                        {
                            if (answer[(i - 1), (j + 1)] != "")
                            {
                                answer[(i - 1), (j + 1)] = answer[(i - 1), (j + 1)][0].ToString() + OppositeStatus(status);
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                        try
                        {
                            if (answer[(i + 1), (j - 1)] != "")
                            {
                                answer[(i + 1), (j - 1)] = answer[(i + 1), (j - 1)][0].ToString() + OppositeStatus(status);
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                        try
                        {
                            if (answer[(i + 1), (j + 1)] != "")
                            {
                                answer[(i + 1), (j + 1)] = answer[(i + 1), (j + 1)][0].ToString() + OppositeStatus(status);
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                        try
                        {
                            if (answer[(i - 1), j] != "")
                            {
                                answer[(i - 1), j] = answer[(i - 1), j][0].ToString() + OppositeStatus(status);
                            }   
                        }
                        catch (System.IndexOutOfRangeException) { }
                        try
                        {
                            if (answer[(i + 1), j] != "")
                            {
                                answer[(i + 1), j] = answer[(i + 1), j][0].ToString() + OppositeStatus(status);
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                        try
                        {
                            if (answer[i, (j - 1)] != "")
                            {
                                answer[i, (j - 1)] = answer[i, (j - 1)][0].ToString() + OppositeStatus(status);
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                        try
                        {
                            if (answer[i, (j + 1)] != "")
                            {
                                answer[i, (j + 1)] = answer[i, (j + 1)][0].ToString() + OppositeStatus(status);
                            }   
                        }
                        catch (System.IndexOutOfRangeException) { }
                        answer[i, j] = "S" + status;
                    }
                }
            }
        }
    }

    void SolveCircle(string[,] answer)
    {
        for (int i = 0; i < answer.GetLength(0); i++)
        {
            for (int j = 0; j < answer.GetLength(1); j++)
            {
                if (answer[i, j] != "")
                {
                    char status = answer[i, j][1];
                    if (answer[i, j][0] == 'C')
                    {
                        //calculate E
                        int CountZero = 0;
                        int CountOne = 0;
                        //up
                        try
                        {
                            if (answer[i - 1, j] != "")
                            {
                                if (answer[i - 1, j][1] == '1')
                                {
                                    CountOne++;
                                }
                                else if (answer[i - 1, j][1] == '0')
                                {
                                    CountZero++; 
                                }
                            } 
                        }
                        catch (System.IndexOutOfRangeException)
                        { }
                        //down
                        try
                        {
                            if (answer[i + 1, j]!="")
                            {
                                if (answer[i + 1, j][1] == '1')
                                {
                                    CountOne++; 
                                }
                                else if (answer[i + 1, j][1] == '0')
                                {
                                    CountZero++;
                                }
                            }
                        }
                        catch (System.IndexOutOfRangeException)
                        { }
                        //left
                        try
                        {
                            if (answer[i, j - 1] != "")
                            {
                                if (answer[i, j - 1][1] == '1')
                                {
                                    CountOne++;
                                }
                                else if (answer[i, j - 1][1] == '0')
                                {
                                    CountZero++;
                                }
                            }
                        }
                        catch (System.IndexOutOfRangeException)
                        { }
                        //right
                        try
                        {
                            if (answer[i, j + 1] != "")
                            {
                                if (answer[i, j + 1][1] == '1')
                                {
                                    CountOne++;
                                    
                                }
                                else if (answer[i, j + 1][1] == '0')
                                {
                                    CountZero++;
                                    
                                }
                            }
                        }
                        catch (System.IndexOutOfRangeException)
                        { }
                        //Status
                        if (CountZero < CountOne) //one
                        {
                            status = '1';
                        }
                        else if (CountZero > CountOne)  //zero
                        {
                            status = '0';
                        }
                        else //opposite
                        {
                            status = OppositeStatus(status);
                        }
                        answer[i, j] = "C" + status;
                    }
                }
            }
        }
    }
    public void CreateAnswer(string[,] answer)
    {
        // Convert the string to a list of characters
        List<char> charList = gamecycler.GameBoard.GetComponent<GeneratorHandler>().ID.ToCharArray().ToList();

        // Sort the list of characters
        charList.Sort();

        // Create a new list with the indices of the characters in the sorted list
        List<int> orderList = charList.Select(c => gamecycler.GameBoard.GetComponent<GeneratorHandler>().ID.IndexOf(c)).ToList();



        //Priority
        foreach (int i in orderList)
        {
            if (i == 1-1)
            {
                SolveLine(answer);
            }
            else if (i == 2 - 1)
            {
                SolveTriangle(answer);
            }
            else if (i == 3 - 1)
            {
                SolveSquare(answer);
            }
            else if (i == 4 - 1)
            {
                SolveCircle(answer);
            }
        }
    }
}
