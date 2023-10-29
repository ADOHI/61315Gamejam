using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    void Start()
    {
        GetComponent<SpriteRenderer>().DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        Transform target = Stage.instance.magicCircles[Stage.instance.currentCircle].transform;
        Vector2 dir = target.position - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
    }
}
