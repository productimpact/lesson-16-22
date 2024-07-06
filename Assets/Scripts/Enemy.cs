using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private BoxCollider _hitZone;
    [SerializeField] private ParticleSystem _bloodEffect;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private int _lives = 100;
    Coroutine attackCoroutine;
    Transform player;
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(_lives > 0 && attackCoroutine == null)
        {
            _animator.SetBool("isRunning", true);
            _agent.SetDestination(player.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackCoroutine());
            }
        }
        if (other.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(30);
            GameObject effect = Instantiate(_bloodEffect.gameObject);
            effect.transform.position = other.transform.position;
            Destroy(effect, _bloodEffect.main.duration);
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if(_lives > 0)
        {
            _lives -= damage;
            if(_lives <= 0)
            {
                Die();
            }
        }
        print(_lives);
    }

    public void Die()
    {
        _animator.SetBool("isDead", true);
        _boxCollider.enabled = false;
        Destroy(gameObject, 3f);
    }

    IEnumerator AttackCoroutine()
    {
        _animator.SetBool("isRunning", false);
        _animator.SetTrigger("Attack");
        yield return new WaitForSeconds(.5f);
        _hitZone.enabled = true;
        _animator.ResetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        _hitZone.enabled = false;
        attackCoroutine = null;
    }
}
