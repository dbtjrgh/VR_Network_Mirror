using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DatabaseUIManager : MonoBehaviour
{
    [Header("============ Panel ============")]
    public GameObject loginPanel;
    public GameObject infoPanel;
    public GameObject updatePanel;

    [Header("============ Input ============")]
    public InputField emailInput;
    public InputField pwInput;
    public InputField nameInput;
    public InputField levelInput;
    public InputField profileTextInput;

    [Header("============ Button ============")]
    public Button signupButton;
    public Button loginButton;
    public Button updateButton;
    public Button deleteButton;
    public Button saveButton;
    public Button cancelButton;

    [Header("============ Text ============")]
    public Text infoText;
    public Text levelText;
    public Text updateText;
    public Text updateNameText;
    public Text updateLevelText;
    public Text updateProfileText;

    private UserData userData;

    private void Awake()
    {
        loginButton.onClick.AddListener(LoginButtonClick);
        signupButton.onClick.AddListener(SignUpButtonClick);
        updateButton.onClick.AddListener(UpdateButtonClick);
        saveButton.onClick.AddListener(SaveButtonClick);
        cancelButton.onClick.AddListener(CancelButtonClick);
        deleteButton.onClick.AddListener(DeleteButtonButtonClick);

    }

    #region 로그인 버튼
    public void LoginButtonClick()
    {
        DatabaseManager.Instance.Login(emailInput.text, pwInput.text, OnLoginSuccess, OnLoginFailure);
    }
    private void OnLoginSuccess(UserData data)
    {
        print("로그인 성공!");
        userData = data;

        loginPanel.SetActive(false);
        infoPanel.SetActive(true);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"안녕하세요, {data.name}");
        sb.AppendLine($"이메일 : {data.email}");
        sb.AppendLine($"직업 : {data.charClass}");
        sb.AppendLine($"소개글 : {data.profileText}");

        infoText.text = sb.ToString();
        levelText.text = $"레벨 : {data.level}";
        StartCoroutine(RoadScene());


    }
    private void OnLoginFailure()
    {
        print("로그인 실패 ㅠㅠ");
    }

    IEnumerator RoadScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Playground");
    }

    #endregion

    #region 회원가입 버튼
    public void SignUpButtonClick()
    {
        DatabaseManager.Instance.Signup(emailInput.text, pwInput.text, SignUpSuccess, SignUpFailure);

    }
    private void SignUpSuccess()
    {
        print("회원가입 성공!");
    }
    private void SignUpFailure()
    {
        print("회원가입 실패! 이메일이 중복됩니다.");
    }
    #endregion

    #region  회원정보 수정 버튼
    private void SaveButtonClick()
    {
        DatabaseManager.Instance.UserUpdate(emailInput.text, nameInput.text, int.Parse(levelInput.text), profileTextInput.text, OnUpdateSuccess, OnUpdateFailure);

    }
    private void CancelButtonClick()
    {
        loginPanel.SetActive(true);
        updatePanel.SetActive(false);
    }

    private void UpdateButtonClick()
    {
        // 로그인 하듯이 이메일과 비밀번호를 입력하고 회원정보 수정 버튼 누르면 해당 로그인 계정의 정보를 불러옴.
        DatabaseManager.Instance.Login(emailInput.text, pwInput.text, OnUpdate, OnUpdateFailure);
    }
    private void OnUpdate(UserData data)
    {
        print("회원정보 열람 성공!");
        userData = data;
        loginPanel.SetActive(false);
        updatePanel.SetActive(true);

        StringBuilder sb = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        StringBuilder sb3 = new StringBuilder();

        sb.AppendLine($"이메일 : {data.email}");
        sb.AppendLine($"이름 :  ");
        sb.AppendLine($"직업 : {data.charClass}");
        sb.AppendLine($"레벨 : ");
        sb.AppendLine($"소개글 :");

        updateText.text = sb.ToString();

        sb1.AppendLine($"{data.name}");
        sb2.AppendLine($"{data.level}");
        sb3.AppendLine($"{data.profileText}");

        updateNameText.text = sb1.ToString();
        updateLevelText.text = sb2.ToString();
        updateProfileText.text = sb3.ToString();
    }
    private void OnUpdateSuccess()
    {
        print("회원정보 업데이트 성공!");
        updatePanel.SetActive(false);
        loginPanel.SetActive(true);
    }
    private void OnUpdateFailure()
    {
        print("회원정보 업데이트 실패!");
    }
    #endregion

    #region 삭제 버튼
    private void DeleteButtonButtonClick()
    {
        DatabaseManager.Instance.Delete(emailInput.text, pwInput.text, DeleteSuccess, DeleteFailure);
    }
    private void DeleteSuccess()
    {
        print("회원삭제 성공!");
    }
    private void DeleteFailure()
    {
        print("회원삭제 실패! 이메일이 틀리거나 비밀번호가 틀립니다.");
    }
    #endregion

    #region 레벨업 버튼
    public void OnLevelButtonClick()
    {
        DatabaseManager.Instance.LevelUp(userData, OnLevelSuccess);
    }

    private void OnLevelSuccess()
    {
        levelText.text = $"레벨 : {userData.level}";
    }
    #endregion
}