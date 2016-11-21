using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace FileSystem
{
    /*インスタンス生成無しで使われるように作成*/
    public class FileManager
    {
        protected string filename;
        protected string filepath;
    }

    /*問題用*/
    public class QuestionFileManager : FileManager
    {
        public QuestionFileManager(string name, string path = "Files/Questions/")
        {
            base.filepath = path;
            base.filename = filepath + name;
        }

        /*ファイルからstringを得る*/
        string LoadStringFromFile()
        {
            TextAsset theTextFile = Resources.Load<TextAsset>(filename);

            if (theTextFile != null)
                return theTextFile.text;

            Debug.Log("File is not exist or empty.");
            return string.Empty;
        }
        /*ファイルから得たsringをList<string>に格納*/
        List<string> SetToStringList(string stringFromFile)
        {
            List<string> vars = new List<string>();

            using (StringReader strReader = new StringReader(stringFromFile))
            {
                string aLine = null;

                while (true)
                {
                    aLine = strReader.ReadLine();

                    if (aLine != null)
                    {
                        vars.Add(aLine);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return vars;
        }

        /*******************************************************/
        /*ファイルから問題を読み込み、questionsリストに格納する*/
        public void SetQuestions(ref List<Question.QuesStructor> question)
        {
            //ファイルからList<string>を得て格納
            List<string> allLines = SetToStringList(LoadStringFromFile());

            for (int i = 0; i < allLines.Count; ++i)
            {
                //現在の行
                string aLine = allLines[i];
                if (aLine[0] == 'Q')
                {
                    try
                    {
                        //問題からずれる行数を数える
                        int cnt = 0;

                        //問題の構造体を作成
                        Question.QuesStructor temp = new Question.QuesStructor();
                        temp.ans = new List<string>();

                        //次の行に該当する要素を当てる
                        temp.type = int.Parse(allLines[i + ++cnt].TrimStart('T'));
                        temp.ques = allLines[i + ++cnt];

                        //答えの数は可変的。Aが出るまで答えとして読み続ける
                        while (allLines[i + cnt + 1][0] != 'A')
                        {
                            temp.ans.Add(allLines[i + ++cnt]);
                        }

                        //正解
                        temp.corAns = int.Parse(allLines[i + (++cnt)].TrimStart('A'));

                        //リストに格納
                        question.Add(temp);
                        i += cnt;
                    }

                    catch (System.ArgumentOutOfRangeException e)
                    {
                        Debug.Log(e);
                        return;
                    }
                }
            }
        }
        /*******************************************************/
    }

    /*点数記録用*/
    public class SaveLoadManager : FileManager
    {
        const int MAXLEVEL = 6;
        enum USER { ID, SCORE, RATE, MAX};

        public SaveLoadManager(string name = "save.txt")
        {
            base.filepath = Application.persistentDataPath + "/save";
            base.filename = filepath + "/" + name;
            Debug.Log(filename);
            InitialzeFile();
        }

        /*IDからScoreを得る*/
        public int GetScoreByID(string id = "default")
        {
            string line;
            int score = -1;

            using (StreamReader theReader = new StreamReader(filename))
            {
                do
                {
                    line = theReader.ReadLine();

                    //特定値獲得
                    if (line == '#' + id)
                    {
                        for (int i = 0; i < (int)USER.SCORE - 1; ++i)
                            theReader.ReadLine();
                        score = int.Parse(theReader.ReadLine());
                        break;
                    }
                } while (theReader.Peek() >= 0);
            }

            //エラー
            if (score == -1)
            {
                AddUser();
                return 0;
                //Debug.Log("SaveLaodManager::GetScoreByID関数から特定値取得に失敗しました。");
            }
            return score;
        }

        /*IDから点数を設定する*/
        public void SetScoreByID(int score, string id = "default")
        {
            string[] lines = System.IO.File.ReadAllLines(filename);

            for(int i = 0; i < lines.Length; ++i)
            {
                if(lines[i] == '#' + id)
                {
                    lines[i + (int)USER.SCORE] = score.ToString();
                    Debug.Log("点数設定成功");
                    break;
                }
            }

            System.IO.File.WriteAllLines(filename, lines);
        }
        
        /*IDからレートを取得*/
        public int GetRateByID(string id = "default")
        {
            string line;
            int rate = -1;

            using (StreamReader theReader = new StreamReader(filename))
            {
                do
                {
                    line = theReader.ReadLine();

                    //特定値獲得
                    if (line == '#' + id)
                    {
                        for (int i = 0; i < (int)USER.RATE - 1; ++i)
                            theReader.ReadLine();

                        rate = int.Parse(theReader.ReadLine()); //rate
                        break;
                    }
                } while (theReader.Peek() >= 0);
            }

            //エラー
            if (rate == -1)
                Debug.Log("SaveLaodManager::GetScoreByID関数から特定値取得に失敗しました。");

            return rate;
        }

        /*点数からレートを設定*/
        public void UpdateRateFromScore(int needScore, string curId = "default")
        {
            //レート上げには不十分
            if (GetScoreByID(curId) < needScore)
                return;
            //挑戦できる最大のレートでないならレート上げはできない
            //if (GetRateByID(curId) != CurrentlyLoginInfo.LEVEL)
            //    return;

            string[] lines = System.IO.File.ReadAllLines(filename);

            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i] == '#' + curId)
                {
                    int curRate = int.Parse(lines[i + (int)USER.RATE]);

                    //システム的に制限値外ではないかチェック
                    if(curRate >= 1 && curRate < MAXLEVEL)
                    {
                        //レート上げ
                        ++curRate;
                        lines[i + (int)USER.RATE] = curRate.ToString();
                    }

                    Debug.Log("レート更新成功");
                    break;
                }
            }

            System.IO.File.WriteAllLines(filename, lines);
        }

        /*登録済みのユーザーならtrueを返す*/
        public bool isExistUser(string ID = "default")
        {
            string[] lines = System.IO.File.ReadAllLines(filename);

            foreach(var line in lines)
            {
                if(line == '#' + ID)
                {
                    return true;
                }
            }

            return false;
        }

        /*新ユーザー登録*/
        public bool AddUser(string id = "default", int score = 0, int rate = 1)
        {
            if (isExistUser(id))
            {
                Debug.Log("既に存在するID");
                return false;
            }

            using (StreamWriter sw = new StreamWriter(filename, true))
            {
                sw.WriteLine('#'+id); // add id line
                sw.WriteLine(score); // add score line
                sw.WriteLine(rate); // add rate line
            }
            return true;
        }

        /*ユーザーを削除する*/
        public void DeleteUser(string ID = "default")
        {
            var lines = new List<string>(System.IO.File.ReadAllLines(filename));
            int deleteLine = -1; // -1から変化なしだったらエラー

            for (int i = 0; i < lines.Count; ++i)
            {
                if(lines[i] == '#' + ID)
                {
                    deleteLine = i;
                }
            }

            //削除処理
            for(int i = 0; i < (int)USER.MAX; ++i)
            {
                lines.RemoveAt(deleteLine);
            }

            File.WriteAllLines(filename, lines.ToArray());
        }

        /*ファイルの存在有無を検査後生成*/
        void InitialzeFile()
        {
            //ディレクトリ生成
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            //ファイル生成
            if (!File.Exists(filename))
            {
                var file = File.Create(filename);
                file.Close();
            }
        }
    }
}