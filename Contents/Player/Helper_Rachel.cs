using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper_Rachel : MonoBehaviour
{
    [Header("# Bash Setting")]
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Animator _anim;
    [SerializeField] private AnimationCurve _bashCurve;

    [SerializeField] private Transform _guantletTransform;
    [SerializeField] private Transform _upperTransform;

    [SerializeField] private float _moveDistance;

    public IEnumerator HelpBash(Vector2 inputVec, float duration)
    {
        Vector2 startPos = gameObject.transform.position;
        Vector2 targetPos = startPos + (inputVec * _moveDistance);

        if (inputVec == Vector2.up)
        {
            _anim.Play("Rachel_Bash_Upper");
        }
        else if (inputVec == Vector2.down)
        {
            _anim.Play("Rachel_Bash_Lower");
        }
        else
        {
            _anim.Play("Rachel_Bash_Front");
            targetPos = startPos + (inputVec * (_moveDistance + 10));
        }

        float timer = 0f;
        float percentage;

        _rigidBody.velocity = Vector2.zero;

        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;

            percentage = timer / duration;

            Vector2 nextPos = Vector2.Lerp(startPos, targetPos, _bashCurve.Evaluate(percentage));
            _rigidBody.MovePosition(nextPos);

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.1f);
    }

    public void Bash_Upper()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/Bash_Upper");

        obj.transform.position = _upperTransform.position;

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        obj.transform.localScale = scale;
    }

    public void Bash_Lower()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/Bash_Lower");

        obj.transform.position = _guantletTransform.position;
    }

    public void Bash_Foward()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/Bash_Foward");

        obj.transform.position = _guantletTransform.position;

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        obj.transform.localScale = scale;

        obj.GetComponent<EffectTracer>().SetTargetAndDamage(_guantletTransform, 0);
    }
}
