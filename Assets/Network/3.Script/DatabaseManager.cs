using MySqlConnector;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

//���� �ٿ�޾Ҵ� ��
//1. database : MariaDB - https://mariadb.org/
//2. db client: HeidiSQL - https://www.heidisql.com/ <-(������DB�� �����Ǿ�������)

public class DatabaseManager : MonoBehaviour
{
    private string serverIP = "127.0.0.1";
    private string dbName = "game";
    private string tableName = "users";
    private string rootPasswd = "tjrgh2";//�׽�Ʈ �ÿ� Ȱ���� �� ������ ���ȿ� ����ϹǷ� ����

    private MySqlConnection conn; //mysql DB�� ������¸� �����ϴ°�ü.

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

    #region �α���
    //�α����� �Ϸ��� �� ��, �α��� ������ ���� ��� �����Ͱ� ���� ���� �� �����Ƿ�,
    //�α����� �Ϸ� �Ǿ��� �� ȣ��� �Լ��� �Ķ���ͷ� �Բ� �޾��ֵ��� ��.
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
            //�α��� ����(email�� pw ���� ���ÿ� ��ġ�ϴ� ���� ������)
            DataRow row = set.Tables[0].Rows[0];
            UserData data = new UserData(row);
            //print(data.email);
            successCallback?.Invoke(data);

        }
        else
        {
            //�α��� ����
            failureCallback?.Invoke();
        }
    }
    #endregion

    #region ȸ������
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

    #region ȸ������ ����
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

    #region ȸ�� Ż��(����)
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

    #region ������
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
            //������ ���������� �����
            data.level = nextLevel;
            successCallback?.Invoke();
        }
        else
        {
            //���� ���� ����
        }
    }
    #endregion
}