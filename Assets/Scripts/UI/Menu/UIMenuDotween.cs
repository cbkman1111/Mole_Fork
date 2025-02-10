using Common.Global;
using Common.Scene;
using Common.UIObject;
using Common.UIObject.Scroll;
using DG.Tweening;
using System;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuDotween : MenuBase
{
    private Action OnShoot = null;
    private Action<float> OnSlide = null;

    [SerializeField]
    private GameObject TextPrefab = null;
    [SerializeField]
    private GameObject spawnPosition = null;

    [SerializeField]
    private TextMeshProUGUI textCombo = null;

    private Pool<Transform> poolText = null;
    private int _score = 0;

    [SerializeField]
    private GameObject PrefabParticle = null;

    public bool InitMenu(Action shoot, Action<float> slide)
    {
        OnShoot = shoot;
        OnSlide = slide;

        _score = 0;
        textCombo.text = _score.ToFormattedString();
        poolText = Pool<Transform>.Create(TextPrefab.transform, transform, 10);
        return true;
    }

    public virtual void OnValueChanged(Slider slider, float f) 
    {
        OnSlide(f);
    }

    protected override void OnClick(Button btn)
    {
        string name = btn.name;
        if (name == "Button - Shoot")
        {
            OnShoot();

            _score++;

            var obj = poolText.GetObject();
            obj.transform.position = spawnPosition.transform.position;
            obj.transform.localScale = Vector3.one;

            var textMesh = obj.GetComponent<TextMeshProUGUI>();
            textMesh.text = _score.ToFormattedString();
            textCombo.text = _score.ToFormattedString();
            var c = textMesh.color;
            c.a = 1;
            textMesh.color = c;

            var sequnce = DOTween.Sequence();
            float intervalMove = 0.6f;

            sequnce.AppendInterval(0.05f);
            sequnce.Join(textMesh.transform.DOMoveY(obj.position.y + 200, intervalMove).SetEase(Ease.Linear));
            sequnce.Join(textMesh.transform.DOShakeScale(0.5f, 1, 10, 90, true).SetEase(Ease.OutBounce));
            sequnce.AppendInterval(0.05f);
            sequnce.Join(textMesh.DOFade(0, 0.3f).SetEase(Ease.Linear));
            sequnce.OnComplete(() =>
            {
                poolText.ReturnObject(obj);
            });
        }
        else if (name == "Button - Back")
        {
            AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
        }
        else if (name == "Button - ParticleLifeCycle")
        {
            var particle = Instantiate<GameObject>(PrefabParticle, null);

        }
    }
}
