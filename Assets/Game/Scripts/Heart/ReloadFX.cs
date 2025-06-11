using System.Collections;
using Febucci.UI;
using UnityEngine;

public class ReloadFX : MonoBehaviour
{
    [SerializeField] private TextAnimator_TMP textAnimator;

    IEnumerator StartTextAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        textAnimator.SetText("<rainb>" + textAnimator.text);
    }

    void OnEnable()
    {
        StartCoroutine(StartTextAfterDelay());
    }

}