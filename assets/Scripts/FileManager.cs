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
        protected string filenameExceptPath;
    }

    /*問題用*/
    public class QuestionFileManager : FileManager
    {
        //問題が出る前に表示されるタイプによって異なる題のこと
        List<string> TypeText;

        public QuestionFileManager(string name, string path = "Files/Questions/")
        {
            filenameExceptPath = name;
            filepath = path;
            filename = filepath + name;
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
        /*問題の個数を求める*/
        public int GetNumOfQuestions()
        {
            //ファイルからList<string>を得て格納
            List<string> allLines = SetToStringList(LoadStringFromFile());
            int num = 0;
            //最初文字がQである文字列を全部探し、数える
            for (int i = 0; i < allLines.Count; ++i)
            {
                string aLine = allLines[i];
                if (aLine[0] == 'Q')
                {
                    ++num;
                }
            }
            return num;
        }
        public int GetNumOfQuestionsofBoss()
        {
            //ボスの問題個数
            QuestionFileManager file = new QuestionFileManager(filenameExceptPath + "Boss");
            return file.GetNumOfQuestions();
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
            question.Clear();
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
        //ファイルに含まれている属性
        public enum PROPERTY_TYPE { POINT, MAX } //ポイント、
        //ファイルに含まれている時間の情報
        public enum TIME_TYPE { YY, MM, DD, hh, mm, ss, MAX} //年、月、日、時、分、秒

        public SaveLoadManager(string name = "save.txt")
        {
            base.filepath = Application.persistentDataPath + "/save";
            base.filename = filepath + "/" + name;
            Debug.Log(filename);
            InitialzeFile();
        }

        /*Scoreを得る*/
        public void GetScore(int rate, ref int score, ref int bossScore)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);

            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i] == '#' + rate.ToString())
                {
                    try
                    {
                        score = int.Parse(lines[i + 1]);
                        bossScore = int.Parse(lines[i + 2]);
                    }
                    catch
                    {
                        AddUser();
                        score = 0;
                        bossScore = 0;
                    }
                    break;
                }
            }
            ////ゲームをやらずに点数参照をする際発生
            //if (rate == 0)
            //{
            //    Debug.Log("難易度が設定されていません。");
            //    return 0;
            //}

            //string line;
            //int score = -1;

            //using (StreamReader theReader = new StreamReader(filename))
            //{
            //    do
            //    {
            //        line = theReader.ReadLine();

            //        //特定値獲得
            //        if (line == '#' + rate.ToString())
            //        {
            //            score = int.Parse(theReader.ReadLine());
            //            break;
            //        }
            //    } while (theReader.Peek() >= 0);
            //}

            ////エラー
            //if (score == -1)
            //{
            //    AddUser();
            //    return 0;
            //    //Debug.Log("SaveLaodManager::GetScoreByID関数から特定値取得に失敗しました。");
            //}

            //return score;
        }
        /*Scoreを設定する*/
        public void SetScore(int rate, int score, int bossScore)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);

            for(int i = 0; i < lines.Length; ++i)
            {
                if(lines[i] == '#' + rate.ToString())
                {
                    lines[i + 1] = score.ToString();
                    lines[i + 2] = bossScore.ToString();
                    Debug.Log("点数設定成功");
                    break;
                }
            }

            System.IO.File.WriteAllLines(filename, lines);
        }
        /*Scoreを足し算する*/
        //public void AddScore(int rate, int addScore)
        //{
        //    SetScore(rate, GetScore(rate) + addScore);
        //}

        /*PROPERTYの値を取得*/
        public int GetNumOfProp(PROPERTY_TYPE property)
        {
            string line;
            int num = -1;

            using (StreamReader theReader = new StreamReader(filename))
            {
                do
                {
                    line = theReader.ReadLine();

                    //特定値獲得
                    if (line == '#' + property.ToString())
                    {
                        num = int.Parse(theReader.ReadLine());
                        break;
                    }
                } while (theReader.Peek() >= 0);
            }

            //エラー
            if (num == -1)
            {
                AddUser();
                return 0;
                //Debug.Log("SaveLaodManager::GetScoreByID関数から特定値取得に失敗しました。");
            }
            return num;
        }
        /*PROPERTYの値を設定*/
        public void SetNumOfProp(PROPERTY_TYPE property, int num)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);

            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i] == '#' + property.ToString())
                {
                    lines[i + 1] = num.ToString();
                    Debug.Log("属性の値設定成功");
                    break;
                }
            }

            System.IO.File.WriteAllLines(filename, lines);
        }
        /*PROPERTYの値を足し算する*/
        public void AddNumOfProp(PROPERTY_TYPE property, int addNum)
        {
            SetNumOfProp(property, GetNumOfProp(property) + addNum);
        }
        
        /*最後の接続時間を取得*/
        public System.DateTime GetPrevTime()
        {
            int YY = -1, MM = 0, DD = 0, hh = 0, mm = 0, ss = 0;
            using (StreamReader theReader = new StreamReader(filename))
            {
                string line;
                do
                {
                    line = theReader.ReadLine();

                    //年度
                    if (line == '#' + TIME_TYPE.YY.ToString())
                    {
                        YY = int.Parse(theReader.ReadLine());
                    }
                    //月
                    else if (line == '#' + TIME_TYPE.MM.ToString())
                    {
                        MM = int.Parse(theReader.ReadLine());
                    }
                    //日
                    else if (line == '#' + TIME_TYPE.DD.ToString())
                    {
                        DD = int.Parse(theReader.ReadLine());
                    }
                    //時
                    else if (line == '#' + TIME_TYPE.hh.ToString())
                    {
                        hh = int.Parse(theReader.ReadLine());
                    }
                    //分
                    else if (line == '#' + TIME_TYPE.mm.ToString())
                    {
                        mm = int.Parse(theReader.ReadLine());
                    }
                    //秒
                    else if (line == '#' + TIME_TYPE.ss.ToString())
                    {
                        ss = int.Parse(theReader.ReadLine());
                    }
                } while (theReader.Peek() >= 0);
            }
            System.DateTime temp;
            //エラー
            if (YY == -1)
            {
                AddUser();
                temp = System.DateTime.Now;
                //Debug.Log("SaveLaodManager::GetScoreByID関数から特定値取得に失敗しました。");
            }
            else
                temp = new System.DateTime(YY, MM, DD, hh, mm, ss);

            return temp;
        }
        /*現在の時間を最後に接続した時間としてファイルにセーブ*/
        public void SetCurrentTime()
        {
            System.DateTime CurDate = System.DateTime.Now;
            SetTime(CurDate);
        }
        /*任意に時間設定*/
        public void SetTime(System.DateTime date)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);

            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i] == '#' + TIME_TYPE.YY.ToString())
                {
                    lines[i + 1] = date.Year.ToString();
                }
                else if (lines[i] == '#' + TIME_TYPE.MM.ToString())
                {
                    lines[i + 1] = date.Month.ToString();
                }
                else if (lines[i] == '#' + TIME_TYPE.DD.ToString())
                {
                    lines[i + 1] = date.Day.ToString();
                }
                else if (lines[i] == '#' + TIME_TYPE.hh.ToString())
                {
                    lines[i + 1] = date.Hour.ToString();
                }
                else if (lines[i] == '#' + TIME_TYPE.mm.ToString())
                {
                    lines[i + 1] = date.Minute.ToString();
                }
                else if (lines[i] == '#' + TIME_TYPE.ss.ToString())
                {
                    lines[i + 1] = date.Second.ToString();
                }
            }

            System.IO.File.WriteAllLines(filename, lines);
        }

        /*現在の時間と最後に接続した時間の間の差*/
        System.TimeSpan GetTimeGap()
        {
            System.DateTime departure = GetPrevTime(); // 最後に接続した日
            System.DateTime arrival = System.DateTime.Now; // 現在
            System.TimeSpan gap = arrival - departure;
            return gap;
        }
        /*何日の時間差かを求める*/
        public int GetTimeGap_Days()
        {
            return GetTimeGap().Days;
        }
        /*接続時間などを基準に、ポイントをアップデートする。出力用の文字列を返す。*/
        public string UpdatePoint_BasedTime()
        {
            /*ポイント計算式：
            * 最後に接続してからの日数がMAX_DAYSより小さい場合は、その日数＋１が点数に加算される。
            * 上の条件を満たさないなら１が点数に加算される。
            */
            const int MAX_DAYS = 5;

            //出力用の文字列
            string DisplayText = "最後の接続から" + GetTimeGap_Days() + "日目\n\n";
            int addPoint = 1;

            if(GetTimeGap_Days() < MAX_DAYS)
            {
                addPoint = MAX_DAYS - GetTimeGap_Days() + 1;
                DisplayText += "POINT +" + addPoint;
            }
            else
            {
                DisplayText += "POINT +1";
            }

            //ファイルに格納
            SetNumOfProp(PROPERTY_TYPE.POINT, GetNumOfProp(PROPERTY_TYPE.POINT) + addPoint);
            DisplayText += "\n\n現在の点数：" + GetNumOfProp(PROPERTY_TYPE.POINT) + "点";

            //出力用の文字列を返す
            return DisplayText;
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
                //点数追加
                for (int i = 0; i < MAXLEVEL; ++i)
                {
                    sw.WriteLine('#' + (i+1).ToString()); // add rate line
                    sw.WriteLine(0); // add score line
                    sw.WriteLine(0); //add boss score line
                }

                //属性追加
                for(int i = 0; i < (int)PROPERTY_TYPE.MAX; ++i)
                {
                    sw.WriteLine('#' + ((PROPERTY_TYPE)i).ToString());
                    sw.WriteLine(0);
                }

                //時間追加
                for (int i = 0; i < (int)TIME_TYPE.MAX; ++i)
                {
                    sw.WriteLine('#' + ((TIME_TYPE)i).ToString());
                    sw.WriteLine(0);
                }
            }
            SetCurrentTime();
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