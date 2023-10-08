using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PinchableScrollView : MonoBehaviour
{
    private RectTransform contentRect;//コンテンツのRectTransformの参照
    private Transform wrapper;			//コンテンツのラッパー
    [SerializeField]
    float scrollSensitivity = 1f;
    [SerializeField]
    private float scale;    //現在の拡大率
    [SerializeField]
    private float TweenSecond;      //収束するまでにかかる時間
    [SerializeField]
    private float RangeScaleMin;          //拡大縮小の範囲
    [SerializeField]
    private float RangeScaleMax;          //拡大縮小の範囲
    [SerializeField]
    private float RangeLimitedScaleMin;   //収束する範囲
    [SerializeField]
    private float RangeLimitedScaleMax;   //収束する範囲
	private Vector3 center;				//現在の中心座標

    // Start is called before the first frame update
    void Start()
    {
        wrapper = transform;
        contentRect=wrapper as RectTransform;
		center = contentRect.localPosition / scale;
		contentRect.anchoredPosition *= scale;
    }

    // Update is called once per frame
    void Update()
    {
        //スクロール終了時の処理
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
        if(Input.GetKeyDown(KeyCode.LeftControl)){
			center = contentRect.localPosition / scale;
        }
    }


    /// <summary>
    /// 新しい拡大率のバリデートと更新をする
    /// </summary>
    private void SetNewScale(float new_scale)
    {
        scale = Mathf.Clamp(new_scale, RangeScaleMin, RangeScaleMax);
    }


    /// <summary>
    /// 収束させる拡大率を求め、コルーチンを開始する
    /// </summary>
    private void StartTweenCoroutine()
    {
        // min < 収束させる拡大率 < max に設定する
        float limited_scale = Mathf.Clamp(scale, RangeLimitedScaleMin, RangeLimitedScaleMax);
        StartCoroutine(TweenLimitedScale(limited_scale));
    }

    /// <summary>
    /// 拡大率を設定された値に収束させる
    /// https://kohki.hatenablog.jp/entry/Unity-uGUI-Pinch-Scaling-forMobile
    /// </summary>
    IEnumerator TweenLimitedScale(float limited_scale)
    {

        if (scale == limited_scale)
            yield break;

        float timer = 0;
        float def_scale = scale - limited_scale;

        //scaleをTweenSecond秒以内にlimited_rateにする
        while (timer < TweenSecond)
        {
            timer += Time.deltaTime;
            scale -= def_scale * Time.deltaTime * (1f / TweenSecond);
            UpdateScaling();
            yield return 0;
        }
    }

    /// <summary>
    /// 設定された拡大率に基づいてオブジェクトの大きさを更新する
    /// </summary>
    private void UpdateScaling()
    {
        //contentRect.localPosition = center * scale;         //拡大率が変わった時に中心座標がずれないように再設定する
        wrapper.localScale = new Vector3(scale, scale, 1);  //全体を拡大縮小する
    }
}
