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
        //問題が出る前に表示されるタイプによって異なる題のこと
        List<string> TypeText;

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
        void SetTypeText()
        {
            TypeText = new List<string>();
            var lines = SetToStringList(LoadStringFromFile());
            for (int i = 0; i < lines.Count; ++i)
            {
                if (lines[i++] == "*SET_TYPE_START")
                {
                    for (int j = i; (lines[j] != "*SET_TYPE_END" && j < lines.Count); ++j)
                    {
                        TypeText.Add(lines[j]);
                    }
                    Debug.Log("タイプ文字取得成功");
                    return;
                }
            }
            Debug.Log("タイプ文字取得失敗");
        }
        public int GetPassScore()
        {
            var lines = SetToStringList(LoadStringFromFile());
            for (int i = 0; i < lines.Count; ++i)
            {
                if (lines[i] == "*PASS_SCORE")
                {
                    Debug.Log("PASS_SCORE取得成功");
                    return int.Parse(lines[i+1].TrimStart('*'));
                }
            }
            Debug.Log("FileSystem::QuestionFileManager::GetPassScore()で値取得失敗");
            return -1;
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
            SetTypeText();
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
                        //タイプ文字
                        temp.typetext = TypeText[temp.type - 1];

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

        public SaveLoadManager(string name = "save.txt")
        {
            base.filepath = Application.persistentDataPath + "/save";
            base.filename = filepath + "/" + name;
            Debug.Log(filename);
            InitialzeFile();
        }

        /*Scoreを得る*/
        public int GetScore(int rate)
        {
            string line;
            int score = -1;

            using (StreamReader theReader = new StreamReader(filename))
            {
                do
                {
                    line = theReader.ReadLine();

                    //特定値獲得
                    if (line == '#' + rate.ToString())
                    {
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

        /*Scoreを設定する*/
        public void SetScore(int score, int rate)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);

            for(int i = 0; i < lines.Length; ++i)
            {
                if(lines[i] == '#' + rate.ToString())
                {
                    lines[i + 1] = score.ToString();
                    Debug.Log("点数設定成功");
                    break;
                }
            }

            System.IO.File.WriteAllLines(filename, lines);
        }
        
        /*レートを取得*/
        public int GetRate()
        {
            string line;
            int myRate = 1;

            using (StreamReader theReader = new StreamReader(filename))
            {
                int rate = 1;
                do
                {
                    line = theReader.ReadLine();
                    
                    //rateの点数をチェック
                    if (line == '#' + rate.ToString())
                    {
                        line = theReader.ReadLine();

                        //該当する問題のファイル
                        QuestionFileManager q = new QuestionFileManager("Lesson" + rate);

                        //点数が合格点を超える
                        if (int.Parse(line) >= q.GetPassScore())
                        {
                            myRate = rate + 1;
                        }
                        ++rate;
                    }

                } while (theReader.Peek() >= 0);
            }

            if (myRate > MAXLEVEL) myRate = MAXLEVEL;
            return myRate;
        }

        /*新ユーザー登録*/
        public bool AddUser()
        {
            //if (isExistUser())
            //{
            //    Debug.Log("既に存在するID");
            //    return false;
            //}

            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                for (int i = 0; i < MAXLEVEL; ++i)
                {
                    sw.WriteLine('#' + (i+1).ToString()); // add rate line
                    sw.WriteLine(0); // add score line
                }
            }
            return true;
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