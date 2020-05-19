using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Player Identification
    [SerializeField] private bool _isPlayerOne = false;
    [SerializeField] private bool _isPlayerTwo = false;

    //Player Speed Variables
    [SerializeField]    private float _normalSpeed = 5.0f;
    [SerializeField]    private float _powerupSpeedMultiplier = 2.0f;
    [SerializeField]    private float _thrusterSpeedMultiplier = 1.5f;
    private float _speed = 0f;

    //Movement Limits
    private float leftHorizontalBoundWrap = -11.25f;
    private float rightHorizontalBoundWrap = 11.25f;
    private float topVerticalBound = 0f;
    private float bottomVerticalBound = -3.8f;

    //Powerup variables
    [SerializeField]    private GameObject _shieldVisual;
    [SerializeField]    private GameObject _leftEngine, _rightEngine;
    [SerializeField] private GameObject[] _shotPrefabs;
    private float _laserOffset = 0.9f;
    [SerializeField]    private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]    private int _lives = 3;
    [SerializeField]    private AudioClip _ammoOutSound;
    private int _shieldStrength = 3;

    //Managers
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameManager _gameManager;

    private bool _isTripleShotPowerupActive = false;
    private bool _isSpeedPowerupActive = false;
    private bool _isShieldPowerupActive = false;
    private bool _isSpeedThrusterActive = false;
    private bool _isMultiShotPowerupActive = false;

    //UI Display
    [SerializeField]    private int _score = 0;
    [SerializeField]    private int _ammoCount = 15;

    private IEnumerator _deactivateSpeedPowerup;
    private Coroutine _shotPowerupRoutine = null;


    private CameraShake _cameraShake;

    private void Awake()
    {
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_cameraShake == null)
        {
            Debug.LogError("Camera Shake instance is NULL!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeVariables();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlayerOne == true)
        {
            if (Input.GetKey(KeyCode.LeftShift) && _isSpeedPowerupActive == false)
                _isSpeedThrusterActive = true;
            else
                _isSpeedThrusterActive = false;

            CalculateMovement();

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && Time.time > _canFire)
            {
                if (_ammoCount <= 0)
                {
                    AudioSource.PlayClipAtPoint(_ammoOutSound, transform.position, 1.0f);
                }
                else
                {
                    FireLaser();
                }
            }
        }

        if (_isPlayerTwo == true)
        {
            //Activate Thruster if Speedup is not active.
            if (Input.GetKey(KeyCode.RightShift) && _isSpeedPowerupActive == false)
                _isSpeedThrusterActive = true;
            else
                _isSpeedThrusterActive = false;


            CalculatePlayerTwoMovement();

            if (Input.GetKeyDown(KeyCode.KeypadEnter) && Time.time > _canFire)
            {
                if (_ammoCount <= 0)
                {
                    AudioSource.PlayClipAtPoint(_ammoOutSound, transform.position, 1.0f);
                }
                else
                {
                    FireLaser();
                }
            }
        }
        if (_ammoCount < 7)     // Generate Ammo powerup when ammo count is lower than 7
        {
            _spawnManager.SetLowAmmo(true);
        }
    }

    private void InitializeVariables()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL");
        }
        if (_rightEngine == null)
        {
            Debug.LogError("Right Engine is NULL");
        }
        if (_leftEngine == null)
        {
            Debug.LogError("Left Engine is NULL");
        }

        if (_gameManager.CoOpMode() == false)
        {
            transform.position = new Vector3(0, 0, 0);
        }

        _leftEngine.SetActive(false);
        _rightEngine.SetActive(false);

        _speed = _normalSpeed;

        UpdateScore(_score);
        UpdateUIAmmoCount(_ammoCount);
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_isSpeedPowerupActive == true)  //Speed up haas priority over Thruster
        {
            _speed = _normalSpeed * _powerupSpeedMultiplier;
        }
        else if (_isSpeedThrusterActive == true)
        {
            _speed = _normalSpeed * _thrusterSpeedMultiplier;
        }
        else
        {
            _speed = _normalSpeed;
        }
        transform.Translate(direction * _speed * Time.deltaTime);

        BoundMovement();
    }

    void CalculatePlayerTwoMovement()
    {
        float speed;
        if (_isSpeedPowerupActive == true)
        {
            speed = _normalSpeed * _powerupSpeedMultiplier;
        }
        else if (_isSpeedThrusterActive == true)
        {
            speed = _normalSpeed * _thrusterSpeedMultiplier;
        }
        else
        {
            speed = _normalSpeed;
        }

        if (Input.GetKey(KeyCode.Keypad8))
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.Keypad6))
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.Keypad4))
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.Keypad2))
            transform.Translate(Vector3.down * speed * Time.deltaTime);

        BoundMovement();
    }

    void BoundMovement()
    {
        //Limit player to go beyond top/bottom limits
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, bottomVerticalBound, topVerticalBound), 0);

        //Wrap Player from right to left or left to right if going beyond limits
        if (transform.position.x > rightHorizontalBoundWrap)
        {
            transform.position = new Vector3(leftHorizontalBoundWrap, transform.position.y, 0);
        }
        else if (transform.position.x < leftHorizontalBoundWrap)
        {
            transform.position = new Vector3(rightHorizontalBoundWrap, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isMultiShotPowerupActive == true)
        {
            Instantiate(_shotPrefabs[2], transform.position, Quaternion.identity);
        }
        else if (_isTripleShotPowerupActive == true)
        {
            Instantiate(_shotPrefabs[1], transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_shotPrefabs[0], transform.position + new Vector3(0, _laserOffset, 0), Quaternion.identity);
        }

        //Counted Triple shot as single ammo, considering Powerup !!
        //Hence ammo count is reduced by 1 out of if structure. 
        //Otherwise can reduce 3 ammo when it's a triple shot, and reduce 1 for regular shot within if condition above.
        UpdateUIAmmoCount(--_ammoCount);
    }

    public void Damage()
    {
        if (_cameraShake != null)
        {
            _cameraShake.ShakeCamera();
        }
        
        if (_isShieldPowerupActive == true)
        {
            _shieldStrength--;
            UpdateShieldStrength();
            return;
        }

        _lives--;

        UpdateLivesStatus();
    }

    public void ActivateSuperShotPowerup(int powerupID)
    {
        _isTripleShotPowerupActive = false;
        _isMultiShotPowerupActive = false;

        if (_shotPowerupRoutine != null)
        {
            StopCoroutine(_shotPowerupRoutine);
        }
        switch (powerupID)
        {
            case 0: //Triple-Shot
                _isTripleShotPowerupActive = true;
                _shotPowerupRoutine = StartCoroutine(DeactivateTripleShotPowerup());
                break;
            case 1: //Multi-Shot
                _isMultiShotPowerupActive = true;
                _shotPowerupRoutine = StartCoroutine(DeactivateMultiShotPowerup());
                break;
            default:
                Debug.Log("New Shot Powerup ID received!! No details registered!");
                break;
        }
    }

    IEnumerator DeactivateTripleShotPowerup()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotPowerupActive = false;
    }

    IEnumerator DeactivateMultiShotPowerup()
    {
        yield return new WaitForSeconds(5.0f);
        _isMultiShotPowerupActive = false;
    }

    public void ActivateSpeedPowerup()
    {
        _isSpeedPowerupActive = true;
        if (_deactivateSpeedPowerup != null)
        {
            StopCoroutine(_deactivateSpeedPowerup);
        }
        _deactivateSpeedPowerup = DeactivateSpeedPowerup();
        StartCoroutine(_deactivateSpeedPowerup);
    }

    IEnumerator DeactivateSpeedPowerup()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedPowerupActive = false;
    }

    public void ActivateShieldPowerup()
    {
        _shieldStrength = 3;
        UpdateShieldStrength();
    }

    public void UpdateShieldStrength()
    {
        switch (_shieldStrength)
        {
            case 3: //Full Strength, Do nothing
                _isShieldPowerupActive = true;
                _shieldVisual.GetComponent<SpriteRenderer>().color = Color.white;
                _shieldVisual.SetActive(true);
                break;
            case 2: //First Hit, change color
                _shieldVisual.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            case 1: //Second Hit, change color
                _shieldVisual.GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case 0: //Third Hit, destroy Shield and reset to original color
                _isShieldPowerupActive = false;
                _shieldStrength = 3;
                _shieldVisual.GetComponent<SpriteRenderer>().color = Color.white;
                _shieldVisual.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void UpdateScore(int updateValue)
    {
        _score += updateValue;
        _uiManager.UpdateUIScoreText(_score);
    }

    public void UpdateUIAmmoCount(int updateValue)
    {
        _uiManager.UpdateUIAmmoCountText(updateValue);
    }

    public void IncreaseAmmoCount(int value)
    {
        _spawnManager.SetLowAmmo(false);
        _ammoCount += value;
        UpdateUIAmmoCount(_ammoCount);
    }

    public void LivesCollected()
    {
        _lives++;
        _spawnManager.SetLowHealth(false);
        UpdateLivesStatus();
    }

    private void UpdateLivesStatus()
    {
        _uiManager.UpdateCurrentLives(_lives);

        if (_lives > 2)
        {
            _leftEngine.SetActive(false);
            _rightEngine.SetActive(false);
        }
        else if (_lives == 2)
        {
            _leftEngine.SetActive(false);
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
            _spawnManager.SetLowHealth(true);
        }

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
}
