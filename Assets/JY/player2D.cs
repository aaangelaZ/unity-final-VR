// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class player2D : MonoBehaviour
// {
//     public string playerWrd = "J";
//     int playerNum = 2;
    

//     int health = 7;

//     int speed = 5;
//     int jumpForce = 700;

//     int bulletForce = 600;

//     bool alive = true;
//     bool hurt = false;
//     public GameObject bulletPrefab;
//     public Transform spawnPoint;

//     Rigidbody2D _rigidbody;
//     Animator _animator;

//     public bool grounded;

//     public LayerMask groundLayer;
//     public Transform feetPos;

//     string attackBtn;
//     string jumpBtn;
//     string xInputAxis;

//     [SerializeField] private AudioSource shootSound;
//     [SerializeField] private AudioSource hurtSound;
//     [SerializeField] private AudioSource playerDeathSound;

//     GameManager _gameManager;

    
//     void Start()
//     {
//         _gameManager = FindObjectOfType<GameManager>();
//         _rigidbody = GetComponent<Rigidbody2D>();
//         _animator = GetComponent<Animator>();

//         attackBtn = "Attack_" + playerWrd;
//         jumpBtn = "Jump_" + playerWrd;
//         xInputAxis = "Horizontal_" + playerWrd;

//     }

//     // Update is called once per frame
//     void Update()
//     {   
//         if(!alive)
//         {
//             return;
//         }
//         float xSpeed = Input.GetAxis(xInputAxis) * speed;
//         _rigidbody.velocity = new Vector2(xSpeed, _rigidbody.velocity.y);
//         _animator.SetFloat("playerspeed", Mathf.Abs(xSpeed));
    
//         if(xSpeed > 0 && transform.localScale.x < 0 ||
//         xSpeed < 0 && transform.localScale.x > 0)
//         {
//             transform.localScale *= new Vector2(-1,1);
//         }

//         if(grounded && Input.GetButtonDown(jumpBtn))
//         {
//             _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
//             _rigidbody.AddForce(new Vector2(0,jumpForce));
//         }

//         if(Input.GetButtonDown(attackBtn))
//         {
//             Vector2 bulletDir = new Vector2(transform.localScale.x,0);
//             bulletDir *= bulletForce;

//             GameObject newBullet = Instantiate(bulletPrefab,spawnPoint.position,Quaternion.identity);
//             newBullet.GetComponent<Rigidbody2D>().AddForce(bulletDir);
            
//             _animator.SetTrigger("Shoot");
//             shootSound.Play();
//         }
//     }

//     private void FixedUpdate() 
//     {
//         grounded = Physics2D.OverlapCircle(feetPos.position, .2f, groundLayer);
//         _animator.SetBool("Grounded", grounded);
//     }

//     private void OnCollisionEnter2D(Collision2D other) 
//     {
//         PlayerHit(other.gameObject);

//         if (other.gameObject.CompareTag("Door"))
//         {
//             SceneManager.LoadScene("level2");
//         }

//     }

//     // private void OnCollisionEnter2D(Collision2D other) 
//     // {
//     //     if (other.gameObject.CompareTag("Door"))
//     //     {
//     //         SceneManager.LoadScene("level2");
//     //     }



//     //     if (alive && !hurt && other.gameObject.CompareTag("Enemy"))
//     //     {
            
//     //         health--;
//     //         if (health < 1)
//     //         {
//     //             _animator.SetTrigger("Die");
//     //             alive = false;
//     //         }
//     //         else
//     //         {
//     //             StartCoroutine(GotHurt());
//     //         }
                
//     //     }
//     private void OnTriggerEnter2D(Collider2D other) 
//     {
//         PlayerHit(other.gameObject);
//     }

//     void PlayerHit(GameObject other)
//     {
//         if(alive && !hurt)
//         {
//             if(other.CompareTag("Enemy"))
//             {
//                 hurtSound.Play();
//                 alive = _gameManager.UpdateHealth(playerNum - 1,5);
//                 PlayerReact(other);
//             }
//              else if(other.CompareTag("Bullet"))
//              {
//                 hurtSound.Play();
//                 alive = _gameManager.UpdateHealth(playerNum - 1,5);
//                  PlayerReact(other);
//              }

//         }

//         void PlayerReact(GameObject other)
//         {
//             // Destroy(other);
//             if(alive)
//             {
//                 StartCoroutine(GotHurt());
//             }
//             else
//             {
//                 playerDeathSound.Play();
//                 _animator.SetTrigger("Die");
//                 Destroy(gameObject, 2);
//             }
//         }

//         IEnumerator GotHurt()
//         {
//             hurt = true;
//             _animator.SetTrigger("Hurt");
//             _rigidbody.AddForce(new Vector2(-transform.localScale.x * 200,200));
//             yield return new WaitForSeconds(.2f);
//             hurt = false;
//         }
//     }    
// }
