using TMPro;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;

public class SubtitleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private float _dissapearAfter = 5f;
    [SerializeField] private float _perCharacterTypeDuration = 0.05f;
    [SerializeField] private float _pauseOnCommaDuration = 0.5f;

    private CancellationTokenSource _cts;
    private Vector2 _originalPosition;
    private CanvasGroup _canvasGroup;

    private void Awake() {
        subtitleText.gameObject.SetActive(false);
        _originalPosition = subtitleText.rectTransform.anchoredPosition;
        _canvasGroup = subtitleText.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    [Button]
    public void TestSubtitle()
    {
        DisplaySubtitle("This is a test subtitle, it should appear and disappear correctly.");
    }

    public void DisplaySubtitle(string subtitle)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        subtitleText.text = string.Empty;
        subtitleText.gameObject.SetActive(true);
        TypeAndHide(subtitle, _cts.Token).Forget();
    }

    private async UniTaskVoid TypeAndHide(string subtitle, CancellationToken token)
    {
        subtitleText.text = string.Empty;
        subtitleText.rectTransform.anchoredPosition = _originalPosition;
        _canvasGroup.alpha = 1f;

        for (int i = 0; i < subtitle.Length; i++)
        {
            token.ThrowIfCancellationRequested();
            subtitleText.text += subtitle[i];
            if (subtitle[i] == ',')
            {
                await UniTask.Delay((int)(_pauseOnCommaDuration * 1000), cancellationToken: token);
            }
            else
            {
                await UniTask.Delay((int)(_perCharacterTypeDuration * 1000), cancellationToken: token);
            }
        }

        await UniTask.Delay((int)(_dissapearAfter * 1000), cancellationToken: token);

        var moveOutTask = LMotion.Create(
            subtitleText.rectTransform.anchoredPosition,
            _originalPosition + new Vector2(0, -30f),
            0.3f
        )
         .BindToAnchoredPosition(subtitleText.rectTransform)
         .ToUniTask(cancellationToken: token);

        var fadeOutTask = LMotion.Create(
            _canvasGroup.alpha,
            0f,
            0.3f
        )
         .BindToAlpha(_canvasGroup)
         .ToUniTask(cancellationToken: token);

        await UniTask.WhenAll(moveOutTask, fadeOutTask);
        subtitleText.text = string.Empty;
        subtitleText.gameObject.SetActive(false);
        subtitleText.rectTransform.anchoredPosition = _originalPosition;
    }
}
