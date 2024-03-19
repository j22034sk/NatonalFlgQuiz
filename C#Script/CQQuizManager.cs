using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CQQuizManager : MonoBehaviour {
    //CSV処理
    public string dataName;//インスペクター上で読み込むファイルの指定
    TextAsset csvFile;
    List<string[]> csvDatas = new List<string[]>(); // CSV読み込み用リスト
    int k = 0;//10問カウンタ
    List<string[]> QuizSelected = new List<string[]>();//選ばれた10問を格納するリスト
    //選択ボタン
    TextMeshProUGUI BTN_text;
    public GameObject QuizBTN_A;
    public GameObject QuizBTN_B;
    public GameObject QuizBTN_C;
    public GameObject QuizBTN_D;
    //スコア処理用
    TextMeshProUGUI Scoretext;
    public GameObject Score;
    int s = 0;
    //問題数処理用
    TextMeshProUGUI qnumtext;
    public GameObject Qnum;
    int qnum = 0;
    //国旗処理用
    RawImage image;
    Texture2D textureimg;
    public GameObject ImgPlate;
    //正誤判定処理用
    string YouAnswer;
    string AnswerTure;
    TextMeshProUGUI TorFtext;
    public GameObject TorF;
    public GameObject TorFPanel;
    //解説テキスト用
    TextMeshProUGUI answer;  
    public GameObject AnswerTXT;//答え
    TextMeshProUGUI explanation01;   
    public GameObject explanationTXT01;//正式名称
    TextMeshProUGUI explanation02;
    public GameObject explanationTXT02;//首都
    TextMeshProUGUI explanation03;
    public GameObject explanationTXT03;//地域
    TextMeshProUGUI explanation04;
    public GameObject explanationTXT04;//解説
    public GameObject AnswerPanel;
    //矢印ボタン用
    public GameObject NextBTN;
    //最終スコア画面
    public GameObject resultPanel;
    TextMeshProUGUI resultscore;  
    public GameObject resultscoreTXT;
    string rank;
    //タイトル画面
    public GameObject firstPanel;
    public GameObject nextBTN;
    public GameObject firstBTN;
    //サウンド
    public AudioClip truese;
    public AudioClip falsese;
    public AudioClip resultse;
    public AudioClip clickse;
    AudioSource audiosource1;
    //ゲーム選択画面への切替
    public GameObject CQquitBTN;
    
    void Start(){
        csvFile = Resources.Load("CSV/Quiz_" + dataName) as TextAsset; //CSV読み込み
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(",")); //","で区切ってリストへ追加
        }

        //最初にコンポーネントを取得(2つの関数・条件分岐で使用するため1回の記述で済むように)
        audiosource1 = GetComponent<AudioSource>();
        TorFtext = TorF.GetComponentInChildren<TextMeshProUGUI>();
        Scoretext = Score.GetComponentInChildren<TextMeshProUGUI>();
        qnumtext = Qnum.GetComponentInChildren<TextMeshProUGUI>();


        firstSet();//スタート画面表示
    }
    //10問選ぶ処理(同じ問題が複数回出題されることを防ぐ)
    void QuizSelectset(){
        List<string[]> shuffledcsvDatas = csvDatas.OrderBy(x => System.Guid.NewGuid()).ToList();//csvDatasをシャッフル
        QuizSelected = shuffledcsvDatas.Take(10).ToList();//10問を別のリストに格納(出題用)
    }
    //国旗をセットする処理
    void QuestionImgSet(){
        //TorFPanel.SetActive(false);
        string quizname = QuizSelected[k][0];
        textureimg = Resources.Load("Img/"+quizname) as Texture2D;//画像読み込み
        image = ImgPlate.GetComponent<RawImage>();
        image.texture = textureimg;//画像表示
        ButtomTXTSet();
    }
    //ボタンのテキスト情報をセットする処理
    void ButtomTXTSet(){
        int r = Random.Range(1,csvDatas.Count - 1);//不正解用データを無作為抽出
        //重複を防ぐ条件分岐    
        while(QuizSelected[k][1] == csvDatas[r-1][1] || QuizSelected[k][1] == csvDatas[r][1] || QuizSelected[k][1] == csvDatas[r+1][1]){
            r = Random.Range(1,csvDatas.Count - 1);//r-1,r,r+1のため範囲を超えないように1～(行数-1)
        }
        //選ばれた4問をリストへ
        string[] array = new string[] { QuizSelected[k][1], csvDatas[r-1][1], csvDatas[r][1], csvDatas[r+1][1] };
        //４問のボタン表示場所をシャッフル
        List<string> shuffledArray = array.OrderBy(x => System.Guid.NewGuid()).ToList();
        BTN_text = QuizBTN_A.GetComponentInChildren<TextMeshProUGUI>();
        BTN_text.text = shuffledArray[0];
        BTN_text = QuizBTN_B.GetComponentInChildren<TextMeshProUGUI>();
        BTN_text.text = shuffledArray[1];
        BTN_text = QuizBTN_C.GetComponentInChildren<TextMeshProUGUI>();
        BTN_text.text = shuffledArray[2];
        BTN_text = QuizBTN_D.GetComponentInChildren<TextMeshProUGUI>();
        BTN_text.text = shuffledArray[3];
        AnswerTXTSet();
    }
    //解説をセットする処理
    void AnswerTXTSet(){
        answer = AnswerTXT.GetComponentInChildren<TextMeshProUGUI>();
        answer.text = "答え:" + QuizSelected[k][1];
        explanation01 = explanationTXT01.GetComponentInChildren<TextMeshProUGUI>();
        explanation01.text ="正式名称:"+ QuizSelected[k][2];
        explanation02 = explanationTXT02.GetComponentInChildren<TextMeshProUGUI>();
        explanation02.text ="首都:" + QuizSelected[k][3];
        explanation03 = explanationTXT03.GetComponentInChildren<TextMeshProUGUI>();
        explanation03.text ="地域:" + QuizSelected[k][4];
        explanation04 = explanationTXT04.GetComponentInChildren<TextMeshProUGUI>();
        explanation04.text = QuizSelected[k][5];
    }
    //最終結果をセットする処理
    public void resultSet(){
        resultPanel.SetActive(true);//最終結果を表示
        audiosource1.PlayOneShot(resultse);
        if(s>8){
            rank = "S";
        }
        else if(s>6){
            rank = "A";
        }
        else if(s>3){
            rank = "B";
        }
        else{
            rank = "C";
        }
        resultscore = resultscoreTXT.GetComponentInChildren<TextMeshProUGUI>();       
        resultscore.text = s + "/10" + "　" + rank; 
    }
    //スタート画面処理
    public void firstSet(){
        firstPanel.SetActive(true);//スタート画面表示
        nextBTN.SetActive(false);//次の問題へ進むボタンを非表示
        resultPanel.SetActive(false);//最終結果非表示
        TorFPanel.SetActive(false);//正誤判定パネル非表示
        AnswerPanel.SetActive(false);//解説パネル非表示
    }
    //押したボタンのテキスト情報を取得する処理
    public void BTNA(){
        YouAnswer = QuizBTN_A.GetComponentInChildren<TextMeshProUGUI>().text;
        BTNclick();
    }
    public void BTNB(){
        YouAnswer = QuizBTN_B.GetComponentInChildren<TextMeshProUGUI>().text;
        BTNclick();
    }
    public void BTNC(){
        YouAnswer = QuizBTN_C.GetComponentInChildren<TextMeshProUGUI>().text;
        BTNclick();
    }
    public void BTND(){
        YouAnswer = QuizBTN_D.GetComponentInChildren<TextMeshProUGUI>().text;
        BTNclick();
    }

    //問題ボタンを押したときの処理
    public void BTNclick(){
        AnswerTure = QuizSelected[k][1];//正解データ
        //押したボタンのデータと正解データの比較で正誤判定
        if(AnswerTure == YouAnswer ){
            audiosource1.PlayOneShot(truese);
            TorFtext.text =  "○正解";
            s = s + 1;
            Scoretext.text = "スコア:"+ s ; 
        }
        else{
            audiosource1.PlayOneShot(falsese);
            TorFtext.text = "×不正解";
        }
        TorFPanel.SetActive(true);//正誤判定パネルの表示
        AnswerPanel.SetActive(true);//解説の表示
        nextBTN.SetActive(true);//次の問題へ進むボタンの表示
    }
    //次の問題、最終結果へ進むボタンを押したときの処理
    public void nextBTNclick(){
        audiosource1.PlayOneShot(clickse);
        qnum += 1 ;//問題数のカウントアップ
        if(qnum == 11){
            resultSet();//11回目で最終結果を表示
        }
        else if(qnum > 11){
            firstSet();//12回目でスタート画面に戻る
        }
        //1～10回では問題数をカウントアップ+問題の更新
        else{
        k += 1 ;//出題用のカウントアップ
        qnumtext.text = "問題数:" + qnum.ToString();
        TorFPanel.SetActive(false);//正誤判定パネルを表示
        AnswerPanel.SetActive(false);//解説パネルを表示
        QuestionImgSet();
        }        
    }
    //ゲームスタートボタンを押したときの処理
    public void startBTNclick(){
        audiosource1.PlayOneShot(clickse);
        qnum = 1;//問題数をリセット
        s = 0;//Scoreをリセット
        k = 0;//10問カウンタをリセット
        qnumtext.text = "問題数:" + qnum.ToString();
        Scoretext.text = "スコア:"+ s ; 
        firstPanel.SetActive(false);//スタート画面非表示
        nextBTN.SetActive(true);//次の問題に進むボタンを表示
        QuizSelectset();
        QuestionImgSet();
    }
    //ゲーム選択シーンへ切り替え
    public void CQquit(){
        SceneManager.LoadScene("GameSelectMenu");
    }
}