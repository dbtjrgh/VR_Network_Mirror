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

    #region �α��� ��ư
    public void LoginButtonClick()
    {
        DatabaseManager.Instance.Login(emailInput.text, pwInput.text, OnLoginSuccess, OnLoginFailure);
    }
    private void OnLoginSuccess(UserData data)
    {
        print("�α��� ����!");
        userData = data;

        loginPanel.SetActive(false);
        infoPanel.SetActive(true);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"�ȳ��ϼ���, {data.name}");
        sb.AppendLine($"�̸��� : {data.email}");
        sb.AppendLine($"���� : {data.charClass}");
        sb.AppendLine($"�Ұ��� : {data.profileText}");

        infoText.text = sb.ToString();
        levelText.text = $"���� : {data.level}";
        StartCoroutine(RoadScene());


    }
    private void OnLoginFailure()
    {
        print("�α��� ���� �Ф�");
    }

    IEnumerator RoadScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Playground");
    }

    #endregion

    #region ȸ������ ��ư
    public void SignUpButtonClick()
    {
        DatabaseManager.Instance.Signup(emailInput.text, pwInput.text, SignUpSuccess, SignUpFailure);

    }
    private void SignUpSuccess()
    {
        print("ȸ������ ����!");
    }
    private void SignUpFailure()
    {
        print("ȸ������ ����! �̸����� �ߺ��˴ϴ�.");
    }
    #endregion

    #region  ȸ������ ���� ��ư
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
        // �α��� �ϵ��� �̸��ϰ� ��й�ȣ�� �Է��ϰ� ȸ������ ���� ��ư ������ �ش� �α��� ������ ������ �ҷ���.
        DatabaseManager.Instance.Login(emailInput.text, pwInput.text, OnUpdate, OnUpdateFailure);
    }
    private void OnUpdate(UserData data)
    {
        print("ȸ������ ���� ����!");
        userData = data;
        loginPanel.SetActive(false);
        updatePanel.SetActive(true);

        StringBuilder sb = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        StringBuilder sb3 = new StringBuilder();

        sb.AppendLine($"�̸��� : {data.email}");
        sb.AppendLine($"�̸� :  ");
        sb.AppendLine($"���� : {data.charClass}");
        sb.AppendLine($"���� : ");
        sb.AppendLine($"�Ұ��� :");

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
        print("ȸ������ ������Ʈ ����!");
        updatePanel.SetActive(false);
        loginPanel.SetActive(true);
    }
    private void OnUpdateFailure()
    {
        print("ȸ������ ������Ʈ ����!");
    }
    #endregion

    #region ���� ��ư
    private void DeleteButtonButtonClick()
    {
        DatabaseManager.Instance.Delete(emailInput.text, pwInput.text, DeleteSuccess, DeleteFailure);
    }
    private void DeleteSuccess()
    {
        print("ȸ������ ����!");
    }
    private void DeleteFailure()
    {
        print("ȸ������ ����! �̸����� Ʋ���ų� ��й�ȣ�� Ʋ���ϴ�.");
    }
    #endregion

    #region ������ ��ư
    public void OnLevelButtonClick()
    {
        DatabaseManager.Instance.LevelUp(userData, OnLevelSuccess);
    }

    private void OnLevelSuccess()
    {
        levelText.text = $"���� : {userData.level}";
    }
    #endregion
}