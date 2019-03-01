using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    // Definir Hashes de:
    // Parametros (Speed, Attack, Damage, Dead)
    int speedHash = Animator.StringToHash("Speed");
    int attackHash = Animator.StringToHash("Attack");
    int damageHash = Animator.StringToHash("Damage");
    int deadHash = Animator.StringToHash("Dead");
    // Estados (Base Layer.Idle, Attack Layer.Idle, Attack Layer.Attack)
    int idleBaseHash = Animator.StringToHash("Base Layer.Idle");
    int idleAttackHash = Animator.StringToHash("Attack Layer.Idle");
    int attackAttackHash = Animator.StringToHash("Attack Layer.Attack");
    // TODO
    Animator anim;
    Rigidbody rb;

	public float walkSpeed		= 1;		// Parametro que define la velocidad de "caminar"
	public float runSpeed		= 1;		// Parametro que define la velocidad de "correr"
	public float rotateSpeed	= 160;		// Parametro que define la velocidad de "girar"

    public AudioClip attackSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
	// Variables auxiliares
	float _angularSpeed			= 0;		// Velocidad de giro actual
	float _speed				= 0;		// Velocidad de traslacion actual
	float _originalColliderZ	= 0;		// Valora original de la posición 'z' del collider

	// Variables internas:
	int _lives = 3;							// Vidas restantes
	public bool paused = false;				// Indica si el player esta pausado (congelado). Que no responde al Input

	void Start()
	{
        // Obtener los componentes Animator, Rigidbody y el valor original center.z del BoxCollider
        // TODO
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        _originalColliderZ = GetComponent<BoxCollider>().center.z;
	}

	// Aqui moveremos y giraremos la araña en funcion del Input
	void FixedUpdate()
	{
		// Si estoy en pausa no hacer nada (no moverme ni atacar)
		if (paused) return;

		// Calculo de velocidad lineal (_speed) y angular (_angularSpeed) en función del Input
		// Si camino/corro hacia delante delante: _speed = walkSpeed   /  _speed = runSpeed
		// TODO
        if (Input.GetKey(KeyCode.UpArrow) || CrossButton.GetInput(InputType.UP))
        {
            if (Input.GetKey(KeyCode.LeftShift) || CrossButton.GetInput(InputType.RUN))
            {
                _speed = runSpeed;
            }
            else
            {
                _speed = walkSpeed;
            }
        }
		// Si camino/corro hacia delante detras: _speed = -walkSpeed   /  _speed = -runSpeed
		// TODO
        else if (Input.GetKey(KeyCode.DownArrow) || CrossButton.GetInput(InputType.DOWN))
        {
            _speed = -walkSpeed;
        }


        // Si no me muevo: _speed = 0
        // TODO
        else
        {
            _speed = 0;
        }
        // Si giro izquierda: _angularSpeed = -rotateSpeed;
        // TODO
        if (Input.GetKey(KeyCode.LeftArrow) || CrossButton.GetInput(InputType.LEFT))
        {
            _angularSpeed = -rotateSpeed;
        }


        // Si giro derecha: _angularSpeed = rotateSpeed;
        // TODO
        else if (Input.GetKey(KeyCode.RightArrow) || CrossButton.GetInput(InputType.RIGHT))
        {
            _angularSpeed = rotateSpeed;
        }
        // Si no giro : _angularSpeed = 0;
        // TODO
        else
        {
            _angularSpeed = 0;
        }
        // Actualizamos el parámetro "Speed" en función de _speed. Para activar la anicación de caminar/correr
        // TODO
        anim.SetFloat(speedHash, _speed);
        // Movemov y rotamos el rigidbody (MovePosition y MoveRotation) en función de "_speed" y "_angularSpeed"
        // TODO
        rb.MovePosition(transform.position + transform.forward * _speed * 0.025f);
        rb.MoveRotation(transform.rotation * Quaternion.Euler(transform.up * _angularSpeed));
        // Mover el collider en función del parámetro "Distance" (necesario cuando atacamos)
        // TODO
        BoxCollider bc = GetComponent<BoxCollider>();
        bc.center = new Vector3(bc.center.x, bc.center.y, anim.GetFloat("Distance") * _originalColliderZ * 20);
    }

	// En este bucle solamente comprobaremos si el Input nos indica "atacar" y activaremos el trigger "Attack"
	private void Update()
	{

        // Si estoy en pausa no hacer nada (no moverme ni atacar)
        // TODO
        if (paused) return;

        // Si detecto Input tecla/boton ataque ==> Activo disparados 'Attack'
        else if (Input.GetKeyDown(KeyCode.Space) || CrossButton.GetInput(InputType.ATTACK))
        {
            anim.SetTrigger(attackHash);
            if (GameManager.instance.soundEnabled)
            {
                GetComponent<AudioSource>().clip = attackSound;
                GetComponent<AudioSource>().Play();
            }
            
        }

    }

	// Función para resetear el Player
	public void reset()
	{
        //Reiniciar el numero de vidas
        // TODO
        _lives = 3;
        // Pausamos a Player
        // TODO
        paused = true;
        // Forzar estado Idle en las dos capas (Base Layer y Attack Layer): función Play() de Animator
        // TODO
        anim.Play(idleBaseHash);
        anim.Play(idleAttackHash);
        // Reseteo todos los triggers (Attack y Dead)
        // TODO
        anim.ResetTrigger(attackHash);
        anim.ResetTrigger(deadHash);
        // Posicionar el jugador en el (0,0,0) y rotación nula (Quaternion.identity)
        // TODO
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
	}

	// Funcion recibir daño
	public void recieveDamage()
	{
        // Restar una vida
        // Si no me quedan vidas notificar al GameManager (notifyPlayerDead) y disparar trigger "Dead"
        // TODO
        _lives -= 1;
        if (GameManager.instance.soundEnabled)
        {
            GetComponent<AudioSource>().clip = damageSound;
            GetComponent<AudioSource>().Play();
        }
        if (_lives == 0)
        {
            GameManager.instance.notifyPlayerDead();
            anim.SetTrigger(deadHash);
            if (GameManager.instance.soundEnabled)
            {
                GetComponent<AudioSource>().clip = deathSound;
                GetComponent<AudioSource>().Play();
            }

        }
        // Si aun me quedan vidas dispara el trigger TakeDamage
        // TODO
        else
        {
            anim.SetTrigger(damageHash);
        }
	}

	private void OnCollisionEnter(Collision collision)
	{
        // Obtener estado actual de la capa Attack Layer
        // TODO
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(1);
        // Si el estado es 'Attack' matamos al enemigo (mirar etiqueta)
        // TODO
        if (stateInfo.fullPathHash == attackAttackHash && collision.gameObject.tag == "Enemy" && anim.GetFloat("Distance")>0.0f)
        {
            collision.gameObject.GetComponent<SkeletonBehaviour>().kill();
        }
    }
}
