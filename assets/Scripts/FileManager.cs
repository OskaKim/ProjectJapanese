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
                        temp.type = int.Parse(allLines[i + ++cnt]);
                        temp.ques = allLines[i + ++cnt];

                        //答えの数は可変的。Aが出るまで答えとして読み続ける
                        while (allLines[i + cnt + 1][0] != 'A')
                        {
                            temp.ans.Add(allLines[i + ++cnt]);
                        }

                        //Aのズレ合わせ
                        ++cnt;

                        //正解
                        temp.corAns = int.Parse(allLines[i + (++cnt)]);

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
        public SaveLoadManager(string name)
        {
            base.filepath = Application.persistentDataPath + "/save";
            base.filename = filepath + "/" + name;
            write();
        }
        void read()
        {
            string line;
            StreamReader theReader = new StreamReader(filename, System.Text.Encoding.Default);
            using (theReader)
            {
                do
                {
                    line = theReader.ReadLine();
                    Debug.Log(line);
                } while (line != null);
            }
        }

        void write()
        {
            //ディレクトリ生成
            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);
            //ファイル生成
            var file = File.Create(filename);
        }
    }
}