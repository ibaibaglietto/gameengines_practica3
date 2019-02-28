using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBehaviour : MonoBehaviour
{
    // Definir Hashes de:
    // Parametros (Attack, Dead, Distance)
    int attackHash = Animator.StringToHash("Attack");
    int damageHash = Animator.StringToHash("Damage");
    int deadHash = Animator.StringToHash("Dead");
    // Estados (Attack, Idle)
    int idleStateHash = Animator.StringToHash("Base Layer.Idle");
    int attackStateHash = Animator.StringToHash("Base Layer.Attack");
    // TODO

    Animator anim;

    // Variables auxiliares 
    PlayerBehaviour _player		= null;     //Puntero a Player (establecido por método 'setPlayer')
	bool _dead					= false;	// Indica si ya he sido eliminado
	float _originalColliderZ	= 0;        // Valora original de la posición 'z' del collider
	float _timeToAttack			= 0;		// Periodo de ataque

	public void setPlayer(PlayerBehaviour player)
	{
		_player = player;
	}

	void Start ()
	{
        // Obtener los componentes Animator y el valor original center.z del BoxCollider
        // TODO
        anim = GetComponent<Animator>();
        _originalColliderZ = GetComponent<BoxCollider>().center.z;
    }
	
	void FixedUpdate ()
	{
        
        // Si estoy muerto ==> No hago nada
        // TODO
        if (_dead) return;
        // Si Player esta a menos de 1m de mi y no estoy muerto:
        // - Le miro
        // - Si ha pasado 1s o más desde el ultimo ataque ==> attack()
        // TODO
        _timeToAttack += Time.fixedDeltaTime;
        //float distPlayer = Mathf.Sqrt(Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + Mathf.Pow(transform.position.z - _player.transform.position.z, 2));
        //if (distPlayer < 1.0f)
        //{
            //transform.LookAt(_player.transform.position);
            if (_timeToAttack > 1.0f)
            {
                attack();
                _timeToAttack = 0.0f;
            }
        //}
        // Desplazar el collider en 'z' un multiplo del parametro Distance
        // TODO
        BoxCollider bc = GetComponent<BoxCollider>();
        bc.center = new Vector3(bc.center.x, bc.center.y, anim.GetFloat("Distance") * _originalColliderZ * 40);

    }

    public void attack()
	{
        // Activo el trigger "Attack"
        // TODO
        anim.SetTrigger(attackHash);
	}

	public void kill()
	{
        // Guardo que estoy muerto, disparo trigger "Dead" y desactivo el collider
        // TODO
        _dead = true;
        anim.SetTrigger(deadHash);
        GetComponent<BoxCollider>().enabled = false;
        // Notifico al GameManager que he sido eliminado
        // TODO
        //GameManager.notifyEnemyKilled(gameObject);
    }

    // Funcion para resetear el collider (activado por defecto), la variable donde almaceno si he muerto y forzar el estado "Idle" en Animator
    public void reset()
	{
        GetComponent<BoxCollider>().enabled = true;
        _dead = true;
        anim.ResetTrigger(deadHash);
        anim.Play(idleStateHash);
    }

	private void OnCollisionEnter(Collision collision)
	{
        // Obtener el estado actual
        // TODO
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        // Si el estado es 'Attack' y el parametro Distance es > 0 atacamos a Player (comprobar etiqueta).
        // La Distancia >0 es para acotar el ataque sólo al momento que mueve la espada (no toda la animación).
        // TODO
        if (stateInfo.fullPathHash == attackStateHash && collision.gameObject.tag == "Player" && anim.GetFloat("Distance") > 0.0f)
        {
            collision.gameObject.GetComponent<PlayerBehaviour>().recieveDamage();
        }
    }
}
