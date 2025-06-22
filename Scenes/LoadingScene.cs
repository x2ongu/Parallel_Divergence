using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScene : BaseScene
{
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private Image _loadingImage;

    static string _nextScene;
    float _z;

    private void Start()
    {
        _z = 0;
        _loadingText.text = "Loading";
        _loadingImage.rectTransform.rotation = Quaternion.Euler(180f, 0f, 0f);

        StartCoroutine(LoadSceneProcess(_nextScene));
    }

    private void Update()
    {
        _z += Time.deltaTime * 50f;
        _loadingImage.rectTransform.rotation = Quaternion.Euler(180f, 0f, _z);
    }

    protected override void Init()
    {
    }

    protected override void OnEnableInit()
    {
    }

    public override void Clear()
    {
        throw new System.NotImplementedException();
    }

    public static void LoadNextScene(string sceneName)
    {
        _nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadSceneProcess(string nextScene)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        yield return new WaitForSeconds(2.5f);

        while (!op.isDone)
        {
            yield return null;

            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.7f);

                op.allowSceneActivation = true;

                break;
            }
        }
    }
}