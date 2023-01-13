using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback, enumSetting
{
    [SerializeField] 
    private Canvas hpBar;
    [SerializeField] 
    private Slider currHpBar;
    [SerializeField] 
    private Image hpFill;
    [SerializeField] 
    private GameObject damageObj;
    [SerializeField] 
    private InputActionAsset inputActionAsset;
    [SerializeField] 
    private List<GameObject> visibleTargets = new List<GameObject>();
    
    private InputActionAsset inputActions;
    private UI ui;
    private Transform camTr;
    private Transform camTp;
    private CharacterController cc;
    private Animator animator;
    private PhotonView pv;
    private GameObject damagePool;
    private GameObject nearObj;

    private PlayerSlotManager slotManager;
    private Team teamObj;

    [SerializeField] private Transform[] skinObj = new Transform[13]; 
    private readonly List <GameObject> skinBelt = new List<GameObject>();
    private readonly List <GameObject> skinCloth = new List<GameObject>();
    private readonly List <GameObject> skinCrown = new List<GameObject>();
    private readonly List <GameObject> skinGlove = new List<GameObject>();
    private readonly List <GameObject> skinHat = new List<GameObject>();
    private readonly List <GameObject> skinHelm = new List<GameObject>();
    private readonly List <GameObject> skinShoe = new List<GameObject>();
    private readonly List <GameObject> skinShoulderPad = new List<GameObject>();
    private readonly List <GameObject> skinHair = new List<GameObject>();
    private readonly List <GameObject> skinHalf = new List<GameObject>();
    [SerializeField] private List <GameObject> skinFace = new List<GameObject>();
    private readonly List <GameObject> skinWeaponLeft = new List<GameObject>();
    private readonly List <GameObject> skinWeaponRight = new List<GameObject>();

    private GameObject[] useWeaponObj = new GameObject[2];
    private CapsuleCollider[] useWeaponCapsule = new CapsuleCollider[2];
    private WeaponData[] useWeaponData = new WeaponData[2];

    private int hairNum = -1;
    private int skinNum = -1;

    private float camMoveSpeed = 3.0f;
    public float camTurnSpeed = 5.0f;
    private readonly short camMinAngle = 10;
    private readonly short camMaxAngle = 70;

    private float zoomSpeed = 5.0f;
    private readonly float minZoom = 0.0f;
    private readonly float maxZoom = 5.0f;
    private readonly float control = 0.01f;
    private readonly float accel = 0.1f;
    private float zoomInput;
    private float zoomRawInput;
    private float zoomDistance;

    private enum CheckState { Move, Run, Ground, Die, Slope, Roof, Jumping, Start, Attack};
    private Dictionary<CheckState, bool> state = new Dictionary<CheckState, bool>() 
    {
        { CheckState.Move, false },
        { CheckState.Run, false },
        { CheckState.Ground, false },
        { CheckState.Slope, false },
        { CheckState.Roof, false},
        { CheckState.Jumping, false},
        { CheckState.Start, false},
        { CheckState.Die, false },
        { CheckState.Attack, true }
    };

    private readonly float roofCrash = 0.0f;
    private readonly float damping = 10.0f;

    private readonly int initHp = 2500;
    [SerializeField] private int currHp = 2500;
    private readonly int initMp = 100;
    [SerializeField] private int currMp = 100;
    [SerializeField] private int armor = 0;
    [SerializeField] private int minDamage = 25;
    [SerializeField] private int maxDamage = 50;
    private readonly float moveInitSpeed = 4.0f;
    [SerializeField] private float moveCurrSpeed;
    private readonly float moveTurnSpeed = 5f;

    private readonly float gravity = Physics.gravity.y;
    private readonly float jumpInitCoolTime = 1f;
    [SerializeField] private float jumpCurrCoolTime;
    private readonly float jumpInitPower = 10f;
    [SerializeField] private float jumpCurrPower;

    private Dictionary<int, float> hit = new Dictionary<int, float>();
    [HideInInspector] public int roomCount;
    private bool isMove = false;
    private bool isRun = false;
    private int attackState = 0;
    private readonly float groundMaxDistance = 0.4f;
    private float groundRawDistance;
    
    private Vector2 mousePos;
    private Vector2 moveInputPos = Vector2.zero;
    private Vector3 movePos; 
    private Vector3 receivePos;
    private Quaternion receiveRot;

    private float _deltaTime;
    private Team team;
    private void Awake()
    {
        InitComponent();
        InitSetting();
        DontDestroyOnLoad(this);
        hpBar.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            Anim();
        }
        hpBar.transform.LookAt(camTr);

    }
    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            // 체크 관련
            UpdateCurrentValues();
            Check();
            Move();
            TpCam();
            Zoom();
            if (slotManager.isStart && SceneManager.GetActiveScene().name != "Slavica Free 2022 Version")
                SceneManager.LoadScene("Slavica Free 2022 Version");
        }
        else
        {
            
            transform.localPosition = Vector3.Lerp(transform.localPosition,
                                                              receivePos,
                                                              Time.deltaTime * damping);
            
            //수신된 회전값으로 보간 회전 처리
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                              receiveRot,
                                                              Time.deltaTime * damping);
        }
    }

    private void InitComponent()
    {
        pv = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        camTr = GameObject.Find("TP_Camera").transform;
        camTp = camTr.parent;
        teamObj = FindObjectOfType<Team>();
        teamObj.player = GetComponent<PlayerControl>();
        inputActions = Instantiate(inputActionAsset);
        DontDestroyOnLoad(inputActions);
        GetComponent<PlayerInput>().actions = inputActions;
        slotManager = FindObjectOfType<PlayerSlotManager>();
        ReadyStart ready = FindObjectOfType<ReadyStart>();
        GameObject obj = PhotonNetwork.IsMasterClient ? ready.btnStart : ready.btnReady;
        obj.SetActive(true);
        InitSkin();
    }

    private void InitSetting()
    {
        zoomDistance = Vector3.Distance(camTp.position, camTr.position);
        moveCurrSpeed = moveInitSpeed;
        currHp = initHp;
        currHpBar.maxValue = initHp;
        currHpBar.value = currHp;
        animator.SetLayerWeight(2, 1);
        UserDataManager manager = UserDataManager.GetInstance();
        var userData = new GetUserDataRequest { PlayFabId = manager.userId };
        var playerProfile = new GetPlayerProfileRequest { PlayFabId = manager.userId };
        PlayFabClientAPI.GetUserData(userData, GetUserData, manager.ErrorMsg);
        PlayFabClientAPI.GetPlayerProfile(playerProfile, GetPlayerProfile, manager.ErrorMsg);

        KeyBinding("Player", "Move");
        KeyBinding("Player", "Dash");
        KeyBinding("Player", "Jump");
        KeyBinding("Player", "Pick");
        KeyBinding("Player", "Inventory");
        KeyBinding("Player", "Character");
        KeyBinding("Player", "Attack");
    }

    private void InitSkin()
    {
        skinSub(skinBelt, 0);
        skinSub(skinCloth, 1);
        skinSub(skinCrown, 2);
        skinSub(skinGlove, 3);
        skinSub(skinHat, 4);
        skinSub(skinHelm, 5);
        skinSub(skinShoe, 6);
        skinSub(skinShoulderPad, 7);
        skinSub(skinHair, 8);
        skinSub(skinHalf, 9);
        skinSub(skinFace, 10);
        skinSub(skinWeaponLeft, 11);
        skinSub(skinWeaponRight, 12);
    }

    private void skinSub(List<GameObject> skinArr, int skinNo)
    {
        for (int idx = 0; idx < skinObj[skinNo].childCount; idx++)
        {
            skinArr.Add(skinObj[skinNo].GetChild(idx).gameObject);
        }
    }
    public override void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "04ReadyRoom" && pv.IsMine)
        {
            pv.RPC(nameof(sceneLoaded), RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    public void sceneLoaded()
    {
        camTr = GameObject.Find("TP_Camera").transform;
        camTp = camTr.parent;
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        var uiObj = PhotonNetwork.Instantiate("Panel - UI", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
        ui = uiObj.GetComponent<UI>();
        ui.transform.SetParent(canvas.transform, false);
        ui.player = GetComponent<PlayerControl>();
        damagePool = GameObject.Find("DamagePanel").gameObject;
        int idx = 0;

        PlayerSpawnPoints spawnPoint = FindObjectOfType<PlayerSpawnPoints>();

        transform.localPosition = spawnPoint.points[idx].localPosition;
    }

    public void SetTagRPC(string str)
    {
        pv.RPC(nameof(Settags), RpcTarget.AllBufferedViaServer, str);
        slotManager.GetTeam(roomCount, true, str);
    }

    [PunRPC]
    public void Settags(string str)
    {
        switch (str)
        {
            case "Red": hpFill.color = Color.red; break;
            case "Orange": hpFill.color = new Color(1, 0.5f, 0, 1); break;
            case "Yellow": hpFill.color = Color.yellow; break;
            case "Green": hpFill.color = Color.green; break;
            case "Blue": hpFill.color = Color.blue; break;
            case "Purple": hpFill.color = new Color(128f, 0f, 128f); break;
        }
        slotManager.slots[roomCount].team.color = hpFill.color;
        tag = str;
    }

    private void KeyBinding(string mapName, string actionName)
    {
        InputAction findAction = inputActions.FindActionMap(mapName).FindAction(actionName);
        if(actionName == "Move")
        {
            KeyOrMouse(findAction, "Up", enumSetting.KeyPart.forward);
            KeyOrMouse(findAction, "Down", enumSetting.KeyPart.backward);
            KeyOrMouse(findAction, "Left", enumSetting.KeyPart.left);
            KeyOrMouse(findAction, "Right", enumSetting.KeyPart.right);
        }
        else
        {
            enumSetting.KeyPart keyPart = (enumSetting.KeyPart)Enum.Parse(typeof(enumSetting.KeyPart), actionName, true);
            KeyOrMouse(findAction, "", keyPart);
        }
    }

    private void KeyOrMouse(InputAction inputAction, string bindingName , enumSetting.KeyPart keyPart)
    {
        KeyCode keyCode = KeySetting.initKey[keyPart];
        string keyMouse = "";
        if(keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1 || keyCode == KeyCode.Mouse2)
        {
            keyMouse = "<Mouse>/";
            switch (keyCode)
            {
                case KeyCode.Mouse0: keyMouse += "leftButton"; break;
                case KeyCode.Mouse1: keyMouse += "rightButton"; break;
                case KeyCode.Mouse2: keyMouse += "middleButton"; break;
            }
        }
        else
        {
            keyMouse = "<Keyboard>/" + KeySetting.initKey[keyPart].ToString();
        }
        if (bindingName == "")
            inputAction.ChangeBinding(0).WithPath(keyMouse);
        else
            inputAction.ChangeBinding(bindingName).WithPath(keyMouse);
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (state[CheckState.Die] == false)
        {
            moveInputPos = input;
            if (context.started)
            {
                isMove = true;
            }
            else if (context.canceled)
            {
                isMove = !(input == Vector2.zero);
            }
            state[CheckState.Move] = !isRun && isMove;
            state[CheckState.Run] = isRun && isMove;
        }
    }

    public void ActionInput(InputAction.CallbackContext context)
    {
        string inputKeyName;
        if (context.control.name == "leftButton")
            inputKeyName = "Mouse0";
        else
            inputKeyName = context.control.name;

        KeyCode keyInput = (KeyCode)Enum.Parse(typeof(KeyCode), inputKeyName, true);

        if (keyInput == KeySetting.initKey[enumSetting.KeyPart.pick])
        {
            if (nearObj != null && context.started)
                pv.RPC(nameof(GetItem), RpcTarget.AllViaServer, null);
        }
        if (keyInput == KeySetting.initKey[enumSetting.KeyPart.attack])
        {
            if (!EventSystem.current.IsPointerOverGameObject() && ui != null)
            {
                if (ui.character.slot[6].weapon != null && context.started)
                {
                    Attack();
                }
            }
        }
        if (keyInput == KeySetting.initKey[enumSetting.KeyPart.dash])
        {
            if (context.started)
                isRun = true;
            else if (context.canceled)
                isRun = false;
            state[CheckState.Move] = !isRun && isMove;
            state[CheckState.Run] = isRun && isMove;
        }
        if (keyInput == KeySetting.initKey[enumSetting.KeyPart.jump])
        {
            if (state[CheckState.Ground] && context.started)
                Jump();
        }

    }
    public void UIInput(InputAction.CallbackContext context)
    {
        KeyCode keyInput = (KeyCode)Enum.Parse(typeof(KeyCode), context.control.name, true);

        if (context.action.triggered && ui != null)
        {
            if (keyInput == KeySetting.initKey[enumSetting.KeyPart.character])
            {
                ui.character.gameObject.SetActive(!ui.character.gameObject.activeSelf);
            }
            else if (keyInput == KeySetting.initKey[enumSetting.KeyPart.inventory])
            {
                ui.inventory.gameObject.SetActive(!ui.inventory.gameObject.activeSelf);
            }
        }
    }
    private void Move()
    {
        if (camTr != null)
        {
            movePos = camTr.rotation * ((Vector3.forward * moveInputPos.y) + (Vector3.right * moveInputPos.x));
            movePos = new Vector3(movePos.x, 0, movePos.z).normalized;
            if ((state[CheckState.Move] || state[CheckState.Run]))
            {
                float angle = Mathf.Atan2(movePos.x, movePos.z) * Mathf.Rad2Deg;
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation,
                                                            Quaternion.Euler(0f, angle, 0f),
                                                            Time.deltaTime * moveTurnSpeed);
            }
            movePos *= (state[CheckState.Run] ? moveCurrSpeed * 2 : moveCurrSpeed);
            movePos.y += jumpCurrPower;
            cc.Move(movePos * _deltaTime);
        }
    }

    private void GetPlayerProfile(GetPlayerProfileResult result)
    {
        if (pv.IsMine)
        {
            for (int i = 0; i < 6; i++)
            {
                if (slotManager.slots[i].order == -1 && slotManager.slots[i].lockObj.activeSelf == false)
                {
                    roomCount = i;
                    slotManager.GetOrder(i, PhotonNetwork.LocalPlayer.ActorNumber);
                    slotManager.GetName(i, result.PlayerProfile.DisplayName);
                    slotManager.GetTeam(i, true, "Red");
                    Settags("Red");
                    slotManager.GetOwner(i, PhotonNetwork.IsMasterClient);
                    slotManager.GetReady(i, PhotonNetwork.IsMasterClient, !PhotonNetwork.IsMasterClient);
                    break;
                }
            }
        }
    }

    private void GetUserData(GetUserDataResult result)
    {
        if (pv.IsMine)
        {
            foreach (var currData in result.Data)
            {
                if (currData.Key == "hairNum")
                    int.TryParse(currData.Value.Value, out hairNum);
                else if (currData.Key == "skinNum")
                    int.TryParse(currData.Value.Value, out skinNum);
            }
            pv.RPC(nameof(SetHair), RpcTarget.AllBufferedViaServer, hairNum);
            pv.RPC(nameof(SetSkin), RpcTarget.AllBufferedViaServer, skinNum);
            StartCoroutine(HairSetting());
        }
    }

    IEnumerator HairSetting()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.5f);
        while (true)
        {
            if (hairNum != -1)
            {
                if (skinHair[hairNum].activeSelf)
                {
                    StopCoroutine(HairSetting());
                    break;
                }
                if(pv.IsMine) pv.RPC(nameof(HairFace), RpcTarget.AllBufferedViaServer);
            }
            yield return wfs;
        }
    }
    [PunRPC] void SetHair(int value) => hairNum = value;
    [PunRPC] void SetSkin(int value) => skinNum = value;
    private void UpdateCurrentValues()
    {
        _deltaTime = Time.smoothDeltaTime;

        if (!state[CheckState.Ground])
            jumpCurrPower += (_deltaTime * gravity);
        else if (state[CheckState.Ground] && jumpCurrCoolTime < jumpInitCoolTime)
            jumpCurrCoolTime += _deltaTime;

        foreach (KeyValuePair<int, float> hits in hit)
        {
            if (hits.Value > 0)
            {
                hit[hits.Key] -= _deltaTime;
            }
            else
            {
                hit.Remove(hits.Key);
            }
        }
    }

    private void Attack()
    {
        if (!(ui.equipment.gameObject.activeSelf || ui.unEquipment.gameObject.activeSelf))
        {
            if(ui.character.slot[6].weapon != null)
            {
                if (ui.character.slot[6].weapon.method == WeaponData.WeaponMethod.Melee)
                    pv.RPC(nameof(Melee), RpcTarget.AllViaServer, state[CheckState.Jumping], state[CheckState.Run]);
                else
                    pv.RPC(nameof(Range), RpcTarget.AllViaServer, state[CheckState.Jumping], state[CheckState.Run]);
            }
        }
    }

    [PunRPC]
    private void AttackCol(bool isActive)
    {
        if (useWeaponCapsule[0] != null)
            useWeaponCapsule[0].enabled = isActive;
        if (useWeaponCapsule[1] != null)
            useWeaponCapsule[1].enabled = isActive;
    }

    [PunRPC]
    private void Melee(bool isJump ,bool isRun)
    {
        if (attackState != 4)
        {
            if (animator.GetBool("FirstAttack"))
            {
                animator.SetBool("isAttack", true);
            }
            else
            {
                animator.SetBool("FirstAttack", true);
                pv.RPC(nameof(AttackCol), RpcTarget.AllViaServer, true);
            }
        }
    }
    [PunRPC]
    private void Range(bool isJump, bool isRun)
    {

    }
    [PunRPC]
    private void GetItem()
    {
        Item item = nearObj.GetComponent<Item>();
        if (item != null)
        {
            ui.inventory.AcquireItem(item.itemData);
            item.Destroy();
        }
    }

    public void StartSkin(ClothData.ClothType cloth, short code, bool isActive)
    {
        if (pv.IsMine && ui != null)
            pv.RPC(nameof(SkinSetting), RpcTarget.AllBufferedViaServer, cloth, code, isActive);
    }

    [PunRPC]
    private void HairFace()
    {
        skinFace[skinNum].SetActive(true);
        skinHair[hairNum].SetActive(true);
    }

    public void startWeapon(short code, bool isLeft, bool isActive)
    {
        if (pv.IsMine)
            pv.RPC("WeaponSetting", RpcTarget.AllBufferedViaServer, code, isLeft, isActive);
    }
    [PunRPC]
    private void SkinSetting(ClothData.ClothType type, short code, bool isActive)
    {
        int num = hairNum;
        switch (type)
        {
            case ClothData.ClothType.Belt:
                skinBelt[code - 1].SetActive(isActive);
                break;
            case ClothData.ClothType.Cloth:
                skinCloth[code - 4].SetActive(isActive);
                skinCloth[0].SetActive(!isActive);
                break;
            case ClothData.ClothType.Glove:
                skinGlove[code - 17].SetActive(isActive);
                skinGlove[0].SetActive(!isActive);
                break;
            case ClothData.ClothType.Shoe:
                skinShoe[code - 34].SetActive(isActive);
                skinShoe[0].SetActive(!isActive);
                break;
            case ClothData.ClothType.ShoulderPad:
                skinShoulderPad[code - 40].SetActive(isActive);
                break;
            case ClothData.ClothType.Crown:
                skinCrown[code - 13].SetActive(isActive);
                break;
            case ClothData.ClothType.Hat:
                skinHat[code - 24].SetActive(isActive);
                if (isActive) skinHalf[num].SetActive(true);
                else skinHair[num].SetActive(false);
                break;
            case ClothData.ClothType.Helm:
                skinHelm[code - 27].SetActive(isActive);
                if (isActive) skinHair[num].SetActive(false);
                else skinHair[num].SetActive(true);
                break;
        }
        StatSetting();
    }
    [PunRPC]
    private void WeaponSetting(short code, bool isLeft, bool isActive)
    {
        int weaponNum = 0;
        int animNum = 0;
        for (int i = 2; i < 7; i++)
        {
            if (animator.GetLayerWeight(i) == 1.0) animator.SetLayerWeight(i, 0);
        }

        switch (code)
        {
            // 도끼
            case 51: weaponNum = 0; break;
            case 52: weaponNum = 1; break;
            // 망치
            case 55: weaponNum = 2; break;
            case 56: weaponNum = 3; break;
            // 검
            case 65: case 66: case 67: case 68: case 69: case 70: case 71:
                weaponNum = code - 61; // 4 ~ 10
                break;
            // 지팡이
            case 72: case 73: case 74:
                weaponNum = code - 61; // 11 ~ 13
                break;
            // 활
            case 53: weaponNum = 14; break;
            case 54: weaponNum = 15; break;
            // 방패
            case 57: case 58: case 59: case 60: case 61: case 62: case 63: case 64:
                weaponNum = code - 41; // 16 ~ 23
                break;
        }
        
        if (isLeft)
           InitWeapon(skinWeaponLeft[weaponNum], 0, isActive);
        else
           InitWeapon(skinWeaponRight[weaponNum], 1, isActive);

        if (ui.character.slot[6].weapon == null)
            animNum = 2;
        else
        {
            switch(ui.character.slot[6].weapon.type)
            {
                case WeaponData.WeaponType.Axe: case WeaponData.WeaponType.Hammer: case WeaponData.WeaponType.Sword:
                    if (ui.character.slot[8].weapon != null)
                    {
                        if (ui.character.slot[8].weapon.type == WeaponData.WeaponType.Shield)
                            animNum = 4;
                        else
                            animNum = 5;
                    }
                    else
                        animNum = 3;
                    break;
                case WeaponData.WeaponType.Bow:
                    animNum = 6;
                    break;
                case WeaponData.WeaponType.Wand:
                    animNum = 7;
                    break;
            }
        }
        animator.SetLayerWeight(animNum, 1);
        StatSetting();
    }

    private void InitWeapon(GameObject obj, int no, bool isActive)
    {
        if (isActive)
        {
            useWeaponObj[no] = obj;
            useWeaponData[no] = obj.GetComponent<Weapon>().data;

            if (useWeaponData[no].method == WeaponData.WeaponMethod.Melee && 
                useWeaponData[no].type != WeaponData.WeaponType.Shield)
                useWeaponCapsule[no] = obj.GetComponent<CapsuleCollider>();

        }
        else
        {
            useWeaponObj[no] = null;
            useWeaponCapsule[no] = null;
            useWeaponData[no] = null;
        }
        obj.SetActive(isActive);
    }

    private void StatSetting()
    {
        int minDam = 25;
        int maxDam = 50;
        int armorNum = 0;
        float speed = 0f;

        for (int i = 0; i < 6; i++)
        {
            if (ui.character.slot[i].cloth != null)
            {
                armorNum += ui.character.slot[i].cloth.Armor;
                speed += ui.character.slot[i].cloth.Speed;
            }
        }
        if (ui.character.slot[6].weapon != null)
        {
            minDam += ui.character.slot[6].weapon.MinDam;
            maxDam += ui.character.slot[6].weapon.MaxDam;
            armorNum += ui.character.slot[6].weapon.Armor;
        }
        if (ui.character.slot[8].weapon != null)
        {
            minDam += ui.character.slot[8].weapon.MinDam;
            maxDam += ui.character.slot[8].weapon.MaxDam;
            armorNum += ui.character.slot[8].weapon.Armor;
        }

        maxDamage = maxDam;
        minDamage = minDam;
        armor = armorNum;
        moveCurrSpeed = moveInitSpeed + speed;
    }

    private void Jump()
    {
        if (jumpCurrCoolTime < jumpInitCoolTime) return;
        jumpCurrPower = jumpInitPower;
        animator.SetTrigger("Jump");
        jumpCurrCoolTime = 0f;
        state[CheckState.Jumping] = true;

    }

    private void Check()
    {
        Vector3 castGround = transform.position + Vector3.up * (cc.height * 0.5f + 0.1f);
        CheckGround(castGround);
        CheckRoof(castGround);
        CheckSlope(castGround);
        CheckCol();
    }
    private void CheckGround(Vector3 castGround)
    {
        bool cast = (Physics.SphereCast(castGround, cc.radius, -transform.up, out var hit,
                                        20f, -1, QueryTriggerInteraction.Ignore));

        if (cast)
        {
            groundRawDistance = hit.distance - (cc.height * 0.2f);
            state[CheckState.Ground] = (groundMaxDistance > groundRawDistance);
            if (state[CheckState.Ground])
                movePos.y = 0;
        }
        else
        {
            state[CheckState.Ground] = false;
            groundRawDistance = 20f;
        }
        state[CheckState.Jumping] = !(cast && state[CheckState.Ground]);

        if (state[CheckState.Ground] && !(jumpInitPower * 0.5f <= jumpCurrPower && jumpCurrPower <= jumpInitPower))
            jumpCurrPower = -5f;
    }
    private void CheckRoof(Vector3 castGround)
    {
        if (state[CheckState.Ground] && !state[CheckState.Jumping]) return;
        state[CheckState.Roof] = Physics.Raycast(castGround, Vector3.up, out var hit, cc.height - 0.1f);

        if (state[CheckState.Roof] && jumpCurrPower > roofCrash)
            jumpCurrPower = roofCrash;
    }

    private void CheckSlope(Vector3 castGround)
    {
        bool isSlope = false;
        isSlope |= Physics.Raycast(castGround, transform.forward, 2f);
        isSlope |= Physics.Raycast(castGround, -transform.forward, 2f);
        isSlope |= Physics.Raycast(castGround, transform.right, 2f);
        isSlope |= Physics.Raycast(castGround, -transform.right, 2f);

        state[CheckState.Slope] = isSlope;
    }

    private void CheckAttack(int idx)
    {
        attackState = idx;
        if (animator.GetBool("isAttack"))
        {
            animator.SetBool("isAttack", false);
        }
        else
        {
            animator.SetBool("FirstAttack", false);
            attackState = 0;
            pv.RPC(nameof(AttackCol), RpcTarget.AllViaServer, false);
        }
    }
    private void CheckCol()
    {
        if (ui == null || !pv.IsMine) return;
        if (ui.character.slot[6].weapon != null)
        {
            if (ui.character.slot[6].weapon.method == WeaponData.WeaponMethod.Melee)
            {
                bool isActive = false;
                if (animator.GetBool("FirstAttack"))
                    isActive = true;
                pv.RPC(nameof(AttackCol), RpcTarget.AllViaServer, isActive);
            }
        }
    }
    private void Anim()
    {
        animator.SetBool("isMoving", state[CheckState.Move]);
        animator.SetBool("isRunning", state[CheckState.Run]);
        animator.SetBool("isGround", state[CheckState.Ground]);
        animator.SetBool("isJumping", state[CheckState.Jumping]);
        animator.SetFloat("Dist", groundRawDistance);
    }

    // 카메라 부분
    private void TpCam()
    {
        if (camTr == null) return;
        camTp.position = Vector3.Lerp(camTp.position, transform.position, camMoveSpeed * _deltaTime);
        bool isCam = true;
        if (ui != null)
        {
            bool isUI = ui.character.gameObject.activeSelf || ui.inventory.gameObject.activeSelf;
            if (isUI && !Input.GetMouseButton(1))
                isCam = false;
        }
        if (isCam)
        {
            mousePos = new Vector2(Input.GetAxis("Mouse X") * camTurnSpeed, Input.GetAxis("Mouse Y") * camTurnSpeed);
            Vector3 cameraPos = camTp.rotation.eulerAngles;
            camTp.rotation = Quaternion.Euler(Mathf.Clamp(cameraPos.x - mousePos.y, camMinAngle, camMaxAngle),
                                                        cameraPos.y + mousePos.x,
                                                        cameraPos.z);
        }
    }

    private void Zoom()
    {
        zoomInput = Input.GetAxisRaw("Mouse ScrollWheel");
        zoomRawInput = Mathf.Lerp(zoomRawInput, zoomInput, accel);
        if (Mathf.Abs(zoomRawInput) < control) return;

        float zom = _deltaTime * zoomSpeed;
        float zoomRawDistance = Vector3.Distance(camTr.position, camTp.position);
        Vector3 move = Vector3.forward * zom;

        if (zoomRawInput > 0.01f && zoomDistance - zoomRawDistance < minZoom)
            camTr.Translate(move, Space.Self);
        else if (zoomRawInput < -0.01f && zoomRawDistance - zoomDistance < maxZoom)
            camTr.Translate(-move, Space.Self);

    }

    private void Damage(Weapon weapon)
    {
        int damInit = UnityEngine.Random.Range(weapon.data.MinDam, weapon.data.MaxDam);
        int damCurr = damInit > armor ? damInit -= armor : 1;
        currHp -= damCurr;
        currHpBar.value = currHp;
        pv.RPC("SetDam", RpcTarget.AllViaServer, damCurr, weapon.tag, transform.position);
        if (currHp <= 0)
        {
            state[CheckState.Die] = true;
            animator.SetTrigger("Die");
        }
    }

    [PunRPC]
    private void SetDam(int dam, string tag, Vector3 pos)
    {
        var obj = Instantiate(damageObj);
        obj.GetComponent<Damage>().isLook(camTr);
        obj.transform.SetParent(damagePool.transform, false);
        obj.transform.position = pos;
        TextMeshProUGUI tm = obj.GetComponent<TextMeshProUGUI>();
        tm.text = dam.ToString();
        foreach (PlayerControl player in FindObjectsOfType<PlayerControl>())
        {
            tm.color = player.CompareTag(tag) ? Color.green : Color.red;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon") && !CompareTag(other.tag))
        {
            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            if (DamCheck(weapon) && !state[CheckState.Die]) Damage(weapon);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
            nearObj = other.gameObject;


    }
    private bool DamCheck(Weapon weapon)
    {
        if (hit.ContainsKey(weapon.player.pv.ViewID))
        {
            return false;
        }
        else
        {
            hit.Add(weapon.player.pv.ViewID, 0.7f);
            return true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        nearObj = null;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(currHpBar.value);
            stream.SendNext(tag);
            stream.SendNext(roomCount);
        }
        else // 데이터 수신
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
            this.currHpBar.value = (float)stream.ReceiveNext();
            this.tag = (string)stream.ReceiveNext();
            this.roomCount = (int)stream.ReceiveNext();
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }
#pragma warning disable CS1717 // 같은 변수에 할당했습니다.
    public void Invoke()
    {
        hairNum = hairNum;
        skinNum = skinNum;
    }
#pragma warning restore CS1717 // 같은 변수에 할당했습니다.
}