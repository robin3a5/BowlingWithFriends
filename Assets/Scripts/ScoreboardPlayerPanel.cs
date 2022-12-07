using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardPlayerPanel : MonoBehaviour
{
    [SerializeField]
    protected TMPro.TMP_Text txtName;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame1;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame2;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame3;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame4;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame5;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame6;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame7;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame8;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame9;

    [SerializeField]
    protected TMPro.TMP_Text txtFrame10;

    [SerializeField]
    protected Button btnKick;

    public event Action OnVoteKickPlayer;

    private bool showVoteKick;

    public void Start()
    {
        btnKick.onClick.AddListener(OnKickPlayerClicked);
    }

    private void OnKickPlayerClicked()
    {
        OnVoteKickPlayer.Invoke();
    }

    public void SetName(string newName)
    {
        txtName.text = newName;
    }

    public string GetName()
    {
        return txtName.text;
    }

    public void SetFrame1(string frameScore)
    {
        txtFrame1.text = frameScore;
    }

    public void SetFrame2(string frameScore)
    {
        txtFrame2.text = frameScore;
    }

    public void SetFrame3(string frameScore)
    {
        txtFrame3.text = frameScore;
    }

    public void SetFrame4(string frameScore)
    {
        txtFrame4.text = frameScore;
    }

    public void SetFrame5(string frameScore)
    {
        txtFrame5.text = frameScore;
    }

    public void SetFrame6(string frameScore)
    {
        txtFrame6.text = frameScore;
    }

    public void SetFrame7(string frameScore)
    {
        txtFrame7.text = frameScore;
    }

    public void SetFrame8(string frameScore)
    {
        txtFrame8.text = frameScore;
    }

    public void SetFrame9(string frameScore)
    {
        txtFrame9.text = frameScore;
    }

    public void SetFrame10(string frameScore)
    {
        txtFrame10.text = frameScore;
    }

    public string GetFrame1()
    {
        return txtFrame1.text;
    }

    public string GetFrame2()
    {
        return txtFrame2.text;
    }

    public string GetFrame3()
    {
        return txtFrame3.text;
    }

    public string GetFrame4()
    {
        return txtFrame4.text;
    }

    public string GetFrame5()
    {
        return txtFrame5.text;
    }

    public string GetFrame6()
    {
        return txtFrame6.text;
    }

    public string GetFrame7()
    {
        return txtFrame7.text;
    }

    public string GetFrame8()
    {
        return txtFrame8.text;
    }

    public string GetFrame9()
    {
        return txtFrame9.text;
    }

    public string GetFrame10()
    {
        return txtFrame10.text;
    }

    public bool GetShowVoteKick()
    {
        return showVoteKick;
    }

    public void ShowKick(bool showKick)
    {
        showVoteKick = showKick;
        if (!showKick)
        {
            btnKick.gameObject.SetActive(false);
        }
    }
}
