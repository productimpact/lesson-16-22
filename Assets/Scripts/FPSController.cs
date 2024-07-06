using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSController : MonoBehaviour
{
    public float movementSpeed = 50.0f;
    public float mouseSensitivity = 2.0f;

    private float verticalRotation = 0;
    private CharacterController characterController;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private TextMeshProUGUI ammoCounter;
    [SerializeField] private TextMeshProUGUI livesCounter;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletPosition;
    private int _actualAmmo = 50;
    private int _lives = 100;
    private int _maxAmmo = 50;
    private bool _isShooting;
    private bool _isReloading;
    Coroutine shootingCoroutine;
    [SerializeField] private float fireDelay = 0.2f;
    [SerializeField] private float reloadDelay = 2f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Image _bloodVignete;
    void Start()
    {
        ammoCounter.text = _actualAmmo + "/50";
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        // Поворот по горизонтали
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, horizontalRotation, 0);

        // Поворот по вертикали
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // Движение
        float forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
        float sideSpeed = Input.GetAxis("Horizontal") * movementSpeed;

        Vector3 speed = new Vector3(sideSpeed, 0, forwardSpeed);
        speed = transform.rotation * speed;

        characterController.SimpleMove(speed);

        // Освобождение курсора при нажатии Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
       
        if (!_isReloading)
        {
            _weapon.transform.rotation = Camera.main.transform.rotation;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _isShooting = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isShooting = false;
        }
        if (_isShooting)
        {
            Fire();
        }
        

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
    }

    void Fire()
    {
        if(shootingCoroutine == null && _actualAmmo > 0)
        {
            Vector3 bulletDirection = bulletPosition.forward;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                bulletDirection = hit.point - bulletPosition.position;
                bulletDirection.Normalize();
            }
            shootingCoroutine = StartCoroutine(FireCoroutine(bulletDirection));
        }
        if(_actualAmmo == 0)
        {
            if (!_isReloading)
            {
                StartCoroutine(ReloadingCoroutine());
            }
        }
    }

    IEnumerator FireCoroutine(Vector3 bulletDirection)
    {
        GameObject b = Instantiate(bullet, bulletPosition);
        b.GetComponent<Bullet>().Init(bulletDirection);
        _actualAmmo--;
        ammoCounter.text = _actualAmmo + "/50";
        audioSource.Play();
        yield return new WaitForSeconds(fireDelay);
        shootingCoroutine = null;
    }

    IEnumerator ReloadingCoroutine()
    {
        _isReloading = true;
        yield return new WaitForSeconds(reloadDelay);
        _actualAmmo = _maxAmmo;
        ammoCounter.text = _actualAmmo + "/50";
        _isReloading = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitZone"))
        {
            _lives -= 10;
            livesCounter.text = _lives.ToString();
            Color color = new Color(1, 1, 1, 1 -_lives / 100f);
            _bloodVignete.color = color;

            if(_lives <= 0)
            {
                enabled = false;
            }
        }
    }
}
