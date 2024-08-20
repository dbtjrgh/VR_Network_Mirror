using MySqlConnector;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

//어제 다운받았던 것
//1. database : MariaDB - https://mariadb.org/
//2. db client: HeidiSQL - https://www.heidisql.com/ <-(마리아DB에 동봉되어있음ㅋ)

public class DatabaseManager : MonoBehaviour
{
    private string serverIP = "127.0.0.1";
    private string dbName = "game";
    private string tableName = "users";
    private string rootPasswd = "tjrgh2";//테스트 시에 활용할 수 있지만 보안에 취약하므로 주의

    private MySqlConnection conn; //mysql DB와 연결상태를 유지하는객체.

    public static DatabaseManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DBConnect();
    }

    public void DBConnect()
    {
        string config = $"server={serverIP};port=3306;database={dbName};" +
            $"uid=root;pwd={rootPasswd};charset=utf8;";

        conn = new MySqlConnection(config);
        conn.Open();
        print(conn.State);
    }

    #region 로그인
    //로그인을 하려고 할 때, 로그인 쿼리를 날린 즉시 데이터가 오지 않을 수 있으므로,
    //로그인이 완료 되었을 때 호출될 함수를 파라미터로 함께 받아주도록 함.
    public void Login(string email, string passwd, Action<UserData> successCallback, Action failureCallback)
    {

        string pwhash = "";

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwd));
            foreach (byte b in hashArray)
            {
                pwhash += $"{b:X2}";
                //pwhash += b.ToString("X2");
            }
        }

        print(pwhash);

        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = conn;
        cmd.CommandText =
            $"SELECT * FROM {tableName} WHERE email='{email}' AND pw='{passwd}'";

        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
        DataSet set = new DataSet();

        dataAdapter.Fill(set);

        bool isLoginSuccess = set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0;

        if (isLoginSuccess)
        {
            //로그인 성공(email과 pw 값이 동시에 일치하는 행이 존재함)
            DataRow row = set.Tables[0].Rows[0];
            UserData data = new UserData(row);
            //print(data.email);
            successCallback?.Invoke(data);

        }
        else
        {
            //로그인 실패
            failureCallback?.Invoke();
        }
    }
    #endregion

    #region 회원가입
    public void Signup(string email, string passwd, Action successCallback, Action failureCallback)
    {
        string pwhash = "";

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwd));
            foreach (byte b in hashArray)
            {
                pwhash += $"{b:X2}";
            }
        }

        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = $"INSERT INTO {tableName}(email,pw,LEVEL,class) VALUES('{email}','{pwhash}',1,1)";

        try
        {
            int result = cmd.ExecuteNonQuery();
            if (result > 0)
            {
                successCallback?.Invoke();
            }
        }
        catch
        {
            failureCallback?.Invoke();
        }
    }
    #endregion

    #region 회원정보 수정
    public void UserUpdate(string email, string name, int level, string profileText, Action successCallback, Action failureCallback)
    {
        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = $"UPDATE {tableName} SET name=@name, LEVEL=@level, profile_text=@profileText WHERE email=@Email";
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@level", level);
        cmd.Parameters.AddWithValue("@profileText", profileText);


        int result = cmd.ExecuteNonQuery();
        if (result > 0)
        {
            successCallback?.Invoke();
        }
        else
        {
            failureCallback?.Invoke();
        }
    }
    #endregion

    #region 회원 탈퇴(삭제)
    public void Delete(string email, string passwd, Action successCallback, Action failureCallback)
    {
        string pwhash = "";

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwd));
            foreach (byte b in hashArray)
            {
                pwhash += $"{b:X2}";
            }
        }

        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = $"DELETE FROM {tableName} WHERE Email = '{email}' AND pw = '{passwd}'";

        try
        {
            int result = cmd.ExecuteNonQuery();
            if (result > 0)
            {
                successCallback?.Invoke();
            }
        }
        catch
        {
            failureCallback?.Invoke();
        }
    }
    #endregion

    #region 레벨업
    public void LevelUp(UserData data, Action successCallback)
    {

        int level = data.level;
        int nextLevel = level + 1;

        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = $"UPDATE {tableName} SET level={nextLevel} WHERE uid={data.UID}";

        int queryCount = cmd.ExecuteNonQuery();

        if (queryCount > 0)
        {
            //쿼리가 정상적으로 수행됨
            data.level = nextLevel;
            successCallback?.Invoke();
        }
        else
        {
            //쿼리 수행 실패
        }
    }
    #endregion
}