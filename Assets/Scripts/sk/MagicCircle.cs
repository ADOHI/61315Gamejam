using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicCircle : MonoBehaviour
{
    int radius = 2;
    int counter = 0;
    public int maxCount = 10;
    bool isActivated = false;
    [System.NonSerialized] public bool isFinished = false;
    [SerializeField] GameObject barBg;
    [SerializeField] Image bar;
    Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated && collision != null)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            Vector2 pos = (Vector2)(transform.position + radius * Random.value * (collision.transform.position - transform.position).normalized);
            enemy.Captured(pos);
            counter++;
            bar.fillAmount = (float)counter / maxCount;
            if (counter >= maxCount)
            {
                isActivated = false;
                isFinished = true;
                Finishted();
            }
        }
    }

    public void Activate()
    {
        isActivated = true;
        animator.SetTrigger("Basic");
        barBg.SetActive(true);
    }

    void Finishted()
    {
        barBg.SetActive(false);
        animator.SetTrigger("On");
        Stage.instance.CircleFinished();
    }
}
