using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PinchableScrollView : MonoBehaviour
{
    private RectTransform contentRect;//�R���e���c��RectTransform�̎Q��
    private Transform wrapper;			//�R���e���c�̃��b�p�[
    [SerializeField]
    float scrollSensitivity = 1f;
    [SerializeField]
    private float scale;    //���݂̊g�嗦
    [SerializeField]
    private float TweenSecond;      //��������܂łɂ����鎞��
    [SerializeField]
    private float RangeScaleMin;          //�g��k���͈̔�
    [SerializeField]
    private float RangeScaleMax;          //�g��k���͈̔�
    [SerializeField]
    private float RangeLimitedScaleMin;   //��������͈�
    [SerializeField]
    private float RangeLimitedScaleMax;   //��������͈�

    // Start is called before the first frame update
    void Start()
    {
        wrapper = transform;
    }

    // Update is called once per frame
    void Update()
    {
        //�X�N���[���I�����̏���
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StartTweenCoroutine();
            return;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log($"scaling... {Input.mouseScrollDelta.y}");
            scale += Input.mouseScrollDelta.y * scrollSensitivity;
            SetNewScale(scale);
            UpdateScaling();
        }
    }


    /// <summary>
    /// �V�����g�嗦�̃o���f�[�g�ƍX�V������
    /// </summary>
    private void SetNewScale(float new_scale)
    {
        scale = Mathf.Clamp(new_scale, RangeScaleMin, RangeScaleMax);
    }


    /// <summary>
    /// ����������g�嗦�����߁A�R���[�`�����J�n����
    /// </summary>
    private void StartTweenCoroutine()
    {
        // min < ����������g�嗦 < max �ɐݒ肷��
        float limited_scale = Mathf.Clamp(scale, RangeLimitedScaleMin, RangeLimitedScaleMax);
        StartCoroutine(TweenLimitedScale(limited_scale));
    }

    /// <summary>
    /// �g�嗦��ݒ肳�ꂽ�l�Ɏ���������
    /// https://kohki.hatenablog.jp/entry/Unity-uGUI-Pinch-Scaling-forMobile
    /// </summary>
    IEnumerator TweenLimitedScale(float limited_scale)
    {

        if (scale == limited_scale)
            yield break;

        float timer = 0;
        float def_scale = scale - limited_scale;

        //scale��TweenSecond�b�ȓ���limited_rate�ɂ���
        while (timer < TweenSecond)
        {
            timer += Time.deltaTime;
            scale -= def_scale * Time.deltaTime * (1f / TweenSecond);
            UpdateScaling();
            yield return 0;
        }
    }

    /// <summary>
    /// �ݒ肳�ꂽ�g�嗦�Ɋ�Â��ăI�u�W�F�N�g�̑傫�����X�V����
    /// </summary>
    private void UpdateScaling()
    {
       // contentRect.localPosition = center * scale;         //�g�嗦���ς�������ɒ��S���W������Ȃ��悤�ɍĐݒ肷��
        wrapper.localScale = new Vector3(scale, scale, 1);  //�S�̂��g��k������
    }
}
