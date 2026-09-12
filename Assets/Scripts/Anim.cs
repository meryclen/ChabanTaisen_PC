using UnityEngine;
using UnityEngine.Animations.Rigging;


public class Anim : MonoBehaviour, ITickable
{
    Animator animator;
    public Animator Animator => animator;

    InputManager inputManager;
    GroundMoter groundMoter;
    Line line;
    [SerializeField] StatusPlayer statusPlayer;
    
    bool isAimState;
    public bool IsAimState => isAimState;
    
    [SerializeField] Rig rig;
    public Rig Rig => rig;
    [SerializeField] MultiParentConstraint hipsConstraint;
    [SerializeField] Transform hipsOffset;
    [SerializeField] TwoBoneIKConstraint rightArmIK;
    public TwoBoneIKConstraint RightArmIK => rightArmIK;
    [SerializeField] TwoBoneIKConstraint leftArmIK;

    [SerializeField] MultiParentConstraint neckConstraint;
    [SerializeField] Transform neckOffset;
    [SerializeField] Transform neckBone;

    [SerializeField] Transform rightArmIKTarget;
    [SerializeField] Transform rightElbowHint;
    [SerializeField] Transform leftArmIKTarget;
    [SerializeField] Transform leftElbowHint;
    [SerializeField] Transform leftFootBone;
    [SerializeField] Transform rightFootBone;

    [SerializeField] float rayOffset = 1f;
    [SerializeField] float rayLength = 1.1f;
    [SerializeField] float rayLength_IkArmFallDown = 1.25f;
    LayerMask groundLayer;

    Transform rightHand;
    Transform rightUpperArm;
    Transform leftHand;
    Transform leftUpperArm;

    float graphics_localPosition_y;

    Quaternion rightHandOffsetConst;
    Quaternion leftHandOffsetConst;
    Quaternion rightFootRotConst;
    Quaternion leftFootRotConst;

    [SerializeField] float armLength = 0.5f;
    Transform gunPivot;
    public Transform GunPivot => gunPivot;
    
    [SerializeField] float armLength_k = 0.001f;
    [SerializeField] float rightElbowDistance = 0.5f;
    [SerializeField] float rightElbowHeight = -1.5f;
    [SerializeField] float leftElbowDistance = 0.5f;
    [SerializeField] float leftElbowHeight = -1.5f;
    [SerializeField] float center_k = 0.001f;
    bool isDeltaY_init = true;
    [SerializeField] float curDeltaY;
    [SerializeField] float deltaY_fadeoutSpeed = 400f;

    Quaternion deltaAngleY;
    float neckAnglelScale = 0.25f;

    [SerializeField] Data left;
    [SerializeField] Data right;

    NeckRnd neckRnd;
    NeckRnd1 neckRnd1;
    NeckRnd2 neckRnd2;
    NeckRnd3 neckRnd3;
    DropRnd[] dropRnd_leftright;
    ArmRnd[] armRnd_leftright;
    FootRnd[] footRnd_leftright;

    int upperBodyLayerIndex;
    public int UpperBodyLayerIndex => upperBodyLayerIndex;
    int fullBodyLayerIndex;
    public int FullBodyLayerIndex => fullBodyLayerIndex;

    [SerializeField] float hipsOffsetY = 0.025f;
    [SerializeField] float lerpSpeed = 5f;

    [SerializeField] float leftRightHandOffsetValue = 0.05f;
    [SerializeField] float targetOffsetY_offset = -0.5f;
    [SerializeField] Quaternion defaultHipsRot;
    [SerializeField] Quaternion runHipsRot = Quaternion.Euler(10f, 0f, 0f);
    [SerializeField] Quaternion runBackHipsRot = Quaternion.Euler(-5f, 0f, 0f);
    [SerializeField] float runHipsSlerpSpeed = 10f;    
    [SerializeField] float leftRightHipsOffsetY = -0.0075f;

    bool isRecoverEnter;
    bool isRecoverExit;
    bool isRecoverLerp;
    bool isRecoverIkCancel;
    public bool IsRecoverIkCancel { set => isRecoverIkCancel = value; }

    [SerializeField] DeadType deadType;
    public DeadType DeadType { get => deadType; set => deadType = value; }
    [SerializeField] SoundPlayer soundPlayer;
    [SerializeField] GroundManager groundManager;
    

    [System.Serializable]
    class Data
    {
        public Transform originFoot;
        public Transform originArmFallDown;
        public Transform originFootFallDown;
        public bool footLock;
        public bool armFallDownLock;
        public bool footFallDownLock;
        public TwoBoneIKConstraint armIk;
        public TwoBoneIKConstraint footIk;
        public Transform footIkTarget;
        public Transform upLeg;
        public Transform kneeHint;
        public Transform elbowHint;
        public Transform armIkTarget;
        public float kneeOffset;
        public float kneeHeight;
        public Quaternion footAxisOffset;
        public float animatorFootIkValue;
        public Vector3 hitFoot;
        public Vector3 hitArmFallDown;
        public Vector3 hitFootFallDown;
    }    


    public void Initialize(InputManager inputManager, GroundMoter groundMoter, Line line)
    {
        this.inputManager = inputManager;
        this.groundMoter = groundMoter;
        this.line = line;
    }
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        rig.weight = 1f;
        left.footIk.weight = 0f;
        right.footIk.weight = 0f;

        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        
        Transform graphics = transform.Find("Graphics");
        graphics_localPosition_y = graphics.localPosition.y;

        GameObject gunPivotGO = GameObject.FindWithTag("GunPivot");
        gunPivot = gunPivotGO.transform;
        groundLayer = LayerMask.GetMask("Ground");

        Vector3 forward =
            Vector3.ProjectOnPlane(
                transform.forward,
                Vector3.up
                ).normalized;
        Quaternion groundRot =
            Quaternion.LookRotation(
                forward,
                Vector3.up
                );
        left.footAxisOffset =
            Quaternion.Inverse(groundRot) *
            leftFootBone.rotation;
        right.footAxisOffset =
            Quaternion.Inverse(groundRot) *
            rightFootBone.rotation;

        line.gameObject.SetActive(false);

        left.footLock = true;
        right.footLock = true;

        upperBodyLayerIndex =
        fullBodyLayerIndex = animator.GetLayerIndex("FullBody");

        Vector3 rightHandOffsetEuler = new Vector3(343.07f, 250.38f, 271.84f);
        rightHandOffsetConst = Quaternion.Euler(rightHandOffsetEuler);
        Vector3 leftHandOffsetEuler = new Vector3(359.26f, 106.76f, 78.55f);
        leftHandOffsetConst = Quaternion.Euler(leftHandOffsetEuler);

        defaultHipsRot = hipsOffset.localRotation;

        rightFootRotConst = Quaternion.Euler(153.5f, 2.9f, 6.4f);
        leftFootRotConst = Quaternion.Euler(153.5f, 2.3f, 5.2f);
        right.footIkTarget.rotation = rightFootRotConst;
        left.footIkTarget.rotation = leftFootRotConst;

        right.footIkTarget.localPosition = new Vector3(0f, -1f, 0f);
        left.footIkTarget.localPosition = new Vector3(0f, -1f, 0f);
        right.originFoot.localPosition = new Vector3(0.05f, 0f, 0f);
        left.originFoot.localPosition = new Vector3(-0.05f, 0f, 0f);
        right.originArmFallDown.localPosition = new Vector3(0.44f, 0f, -0.15f);
        left.originArmFallDown.localPosition = new Vector3(-0.44f, 0f, -0.15f);
        right.originFootFallDown.localPosition = new Vector3(0.1f, 0f, 0.675f);
        left.originFootFallDown.localPosition = new Vector3(-0.1f, 0f, 0.675f);


        neckRnd = new NeckRnd(15f);        
        neckRnd1 = new NeckRnd1(15f);
        neckRnd2 = new NeckRnd2(8f);
        neckRnd3 = new NeckRnd3(8f);

        dropRnd_leftright = new DropRnd[2];
        for (int i=0; i<dropRnd_leftright.Length; i++)
        {
            dropRnd_leftright[i] = new DropRnd(0.1f, 15f);
        }

        armRnd_leftright = new ArmRnd[2];
        for (int i=0; i<armRnd_leftright.Length; i++)
        {
            armRnd_leftright[i] = new ArmRnd(0.03f);
        }

        footRnd_leftright = new FootRnd[2];
        for (int i=0; i<footRnd_leftright.Length; i++)
        {
            footRnd_leftright[i] = new FootRnd(0.05f);
        }
    }

    public void Tick()
    {
        if (inputManager.Move.y > 0f) animator.SetFloat("moveY", 1f);
        if (inputManager.Move.y == 0f) animator.SetFloat("moveY", 0f);
        if (inputManager.Move.y < 0f) animator.SetFloat("moveY", -1f);

        if (inputManager.Move.x < 0f) animator.SetFloat("moveX", -1f);
        if (inputManager.Move.x == 0f) animator.SetFloat("moveX", 0f);
        if (inputManager.Move.x > 0f) animator.SetFloat("moveX", 1f);

        left.animatorFootIkValue = animator.GetFloat("leftFootIK");
        right.animatorFootIkValue = animator.GetFloat("rightFootIK");        
    }

    public void LateTick()
    {
        Quaternion baseRot = transform.rotation;
        Vector3 transformUp = transform.up;
        Vector3 transformRight = transform.right;

        if (groundMoter.gravityDamageInfo.DamageType == DamageType.None &&
            statusPlayer.PlayerStatus != PlayerStatus.Dead &&
            statusPlayer.PlayerStatus != PlayerStatus.FallDown &&
            statusPlayer.PlayerStatus != PlayerStatus.Recover)
        {
            if (isAimState)
            {
                if (rightArmIK.weight != 1) rightArmIK.weight = 1f;
                if (leftArmIK.weight != 1) leftArmIK.weight = 1f;

                deltaAngleY = Quaternion.AngleAxis(inputManager.deltaY, Vector3.right);

                gunPivot.rotation = baseRot * deltaAngleY;
                Vector3 tmp = rightUpperArm.position +
                    gunPivot.forward * (armLength - (75f - inputManager.deltaY) * armLength_k) -
                    transformRight * 75f * center_k;
                tmp.y -= graphics_localPosition_y;

                rightArmIKTarget.position = tmp;
                Vector3 leftRightOffset = -transform.right * leftRightHandOffsetValue;
                leftArmIKTarget.position = (tmp += leftRightOffset);

                rightArmIKTarget.rotation = gunPivot.rotation * rightHandOffsetConst;
                leftArmIKTarget.rotation = gunPivot.rotation * leftHandOffsetConst;

                Vector3 dir_rightUpperToHand =
                    (rightHand.position - rightUpperArm.position).normalized;
                Vector3 side_rightUpperToHand =
                    Vector3.Cross(transformUp, dir_rightUpperToHand).normalized;

                rightElbowHint.position =
                    rightUpperArm.position +
                    side_rightUpperToHand * rightElbowDistance +
                    transformUp * rightElbowHeight;

                Vector3 dir_leftUpperToHand =
                    (leftHand.position - leftUpperArm.position).normalized;
                Vector3 side_leftUpperToHand =
                    Vector3.Cross(transformUp, dir_leftUpperToHand).normalized;

                leftElbowHint.position =
                    leftUpperArm.position +
                    side_leftUpperToHand * leftElbowDistance +
                    transformUp * leftElbowHeight;
            }
            else if (!animator.GetBool("isAiming"))
            {                
                if (!isDeltaY_init)
                {
                    isDeltaY_init = true;
                    curDeltaY = inputManager.deltaY;
                }
                
                if (!animator.GetBool("isAimFinished") && curDeltaY >= 75f)
                {
                    animator.SetBool("isAimFinished", true);
                    rightArmIK.weight = 0f;
                    leftArmIK.weight = 0f;
                }

                curDeltaY = Mathf.MoveTowards(curDeltaY, 75f, deltaY_fadeoutSpeed * Time.deltaTime);
                deltaAngleY = Quaternion.AngleAxis(curDeltaY, Vector3.right);
                gunPivot.rotation = baseRot * deltaAngleY;
                Vector3 tmp = rightUpperArm.position +
                    gunPivot.forward * (armLength - (75f - curDeltaY) * armLength_k) -
                    transformRight * 75f * center_k;
                tmp.y -= graphics_localPosition_y;

                rightArmIKTarget.position = tmp;
                rightArmIKTarget.rotation = gunPivot.rotation * rightHandOffsetConst;
                leftArmIKTarget.rotation = gunPivot.rotation * leftHandOffsetConst;
                Vector3 leftRightOffset = -transform.right * leftRightHandOffsetValue;
                leftArmIKTarget.position = (tmp += leftRightOffset);

                Vector3 dir_rightUpperToHand =
                    (rightHand.position - rightUpperArm.position).normalized;
                Vector3 side_rightUpperToHand =
                    Vector3.Cross(transformUp, dir_rightUpperToHand).normalized;

                rightElbowHint.position =
                    rightUpperArm.position +
                    side_rightUpperToHand * rightElbowDistance +
                    transformUp * rightElbowHeight;

                Vector3 dir_leftUpperToHand =
                    (leftHand.position - leftUpperArm.position).normalized;
                Vector3 side_leftUpperToHand =
                    Vector3.Cross(transformUp, dir_leftUpperToHand).normalized;

                leftElbowHint.position =
                    leftUpperArm.position +
                    side_leftUpperToHand * leftElbowDistance +
                    transformUp * leftElbowHeight;
            }
            else
            {

            }
        }


        if (isAimState)
        {
            Quaternion targetRot = Quaternion.Euler(inputManager.deltaY * neckAnglelScale, 0f, 0f);
            neckOffset.localRotation =
                Quaternion.Slerp(
                    neckOffset.localRotation,
                    targetRot,
                    30f * Time.deltaTime);

            if (statusPlayer.PlayerStatus == PlayerStatus.Dead)
            {
                if (left.armIk.weight == 0f) left.armIk.weight = 1f;
                if (right.armIk.weight == 0f) right.armIk.weight = 1f;
            }
        }


        float targetHip = Mathf.Min(left.upLeg.position.y, right.upLeg.position.y);
        Vector3 pos = hipsOffset.localPosition;

        if (statusPlayer.PlayerStatus != PlayerStatus.Dead &&
            statusPlayer.PlayerStatus != PlayerStatus.FallDown &&
            statusPlayer.PlayerStatus != PlayerStatus.Recover &&
            groundMoter.gravityDamageInfo.DamageType != DamageType.Falling)
        {
            if (left.footLock || right.footLock)
            {
                float leftOffsetY = left.originFoot.position.y - left.footIkTarget.position.y;
                float rightOffsetY = right.originFoot.position.y - right.footIkTarget.position.y;
                float targetOffsetY = Mathf.Min(leftOffsetY, rightOffsetY);
                targetOffsetY -= graphics_localPosition_y;
                targetOffsetY += targetOffsetY_offset;

                Vector3 tmp = hipsOffset.localPosition;
                float curY = tmp.y;
                tmp.y = Mathf.Lerp(
                    curY,
                    -targetOffsetY,
                    lerpSpeed * Time.deltaTime);
                hipsOffset.localPosition = tmp;
            }
            else
            {
                Vector3 tmp = hipsOffset.localPosition;
                float curY = tmp.y;
                tmp.y = Mathf.Lerp(
                    curY,
                    -hipsOffsetY,
                    lerpSpeed * Time.deltaTime);
                hipsOffset.localPosition = tmp;
            }
        }


        if (DeadType == DeadType.DropOut)
        {
            statusPlayer.RaiseDropOut();

            for (int i = 0; i < dropRnd_leftright.Length; i++)
            {
                if (!dropRnd_leftright[i].IsGenerate)
                {
                    dropRnd_leftright[i].IsGenerate = true;

                    if (i == 0)
                    {
                        dropRnd_leftright[i].IkTargetStartPos = left.armIkTarget.localPosition;
                        dropRnd_leftright[i].IkTargetStartRot = left.armIkTarget.localRotation;
                        left.elbowHint.localPosition = new Vector3(-0.59f, -0.96f, -0.0379f);
                    }
                    if (i == 1)
                    {
                        dropRnd_leftright[i].IkTargetStartPos = right.armIkTarget.localPosition;
                        dropRnd_leftright[i].IkTargetStartRot = right.armIkTarget.localRotation;
                        right.elbowHint.localPosition = new Vector3(0.673f, -0.96f, -0.0379f);
                    }

                    dropRnd_leftright[i].TargetPosGenerate();
                    dropRnd_leftright[i].TargetRotGenerate();
                    ArmIkOn();
                    Log.D("ArmIkOn()");
                }
                if (i == 0)
                {
                    left.armIkTarget.localPosition =
                        Vector3.Lerp(
                            left.armIkTarget.localPosition,
                            dropRnd_leftright[i].TargetPos,
                            3f * Time.deltaTime);
                    left.armIkTarget.localRotation =
                        Quaternion.Slerp(
                            left.armIkTarget.localRotation,
                            Quaternion.Euler(0f, 60f, 0f),
                            3f * Time.deltaTime);
                }
                if (i == 1)
                {
                    right.armIkTarget.localPosition =
                        Vector3.Lerp(
                            right.armIkTarget.localPosition,
                            dropRnd_leftright[i].TargetPos,
                            3f * Time.deltaTime);
                    right.armIkTarget.localRotation =
                        Quaternion.Slerp(
                            right.armIkTarget.localRotation,
                            Quaternion.Euler(0f, -60f, 0f),
                            3f * Time.deltaTime);
                }
            }

            if (!neckRnd1.IsGenerate)
            {
                neckRnd1.IsGenerate = true;
                neckRnd1.NeckBoneParentRotation = neckBone.parent.rotation;
                neckRnd1.TargetRotGenerate();
            }
            neckOffset.rotation =
                Quaternion.Slerp(
                    neckOffset.rotation,
                    neckRnd1.TargetRot,
                    5f * Time.deltaTime);
        }
        else if (statusPlayer.PlayerStatus == PlayerStatus.Dead ||
            statusPlayer.PlayerStatus == PlayerStatus.FallDown &&
            groundMoter.gravityDamageInfo.ShockPower > 200)
        {
            hipsOffset.localRotation =
                Quaternion.Slerp(
                    hipsOffset.localRotation,
                    Quaternion.AngleAxis(-90f, Vector3.right),
                    30f * Time.deltaTime);
            if (statusPlayer.PlayerStatus == PlayerStatus.Dead &&
                Mathf.Abs(Quaternion.Angle(hipsOffset.localRotation,
                Quaternion.AngleAxis(-90f, Vector3.right))) < 0.01f)
            {
                neckRnd.IsRotStart = true;
            }
            
            Vector3 tmpLocalPosition = hipsOffset.localPosition;
            tmpLocalPosition.y =
                Mathf.Lerp(
                    tmpLocalPosition.y,
                    -1f,
                    2f * lerpSpeed * Time.deltaTime);
            hipsOffset.localPosition = tmpLocalPosition;

            if (statusPlayer.PlayerStatus == PlayerStatus.Dead && neckRnd.IsRotStart)
            {
                if (!neckRnd.IsGenerate)
                {
                    neckRnd.IsGenerate = true;
                    neckRnd.NeckBoneParentRotation = neckBone.parent.rotation;
                    neckRnd.TargetRotGenerate();
                }
                neckOffset.rotation =
                    Quaternion.Slerp(
                        neckOffset.rotation,
                        neckRnd.TargetRot,
                        20f * Time.deltaTime);
            }
        }
        else if (statusPlayer.PlayerStatus == PlayerStatus.Recover)
        {
            if (!neckRnd2.IsGenerate)
            {
                neckRnd2.IsGenerate = true;
                neckOffset.localRotation = Quaternion.identity;
                neckRnd2.TargetRotGenerate();
                neckRnd2.IsRotStart = true;
            }
            if (neckRnd2.IsRotStart)
            {
                neckOffset.localRotation =
                    Quaternion.Slerp(
                        neckOffset.localRotation,
                        neckRnd2.LocalRot,
                        15f * Time.deltaTime);
            }
            if (Mathf.Abs(Quaternion.Angle(neckOffset.localRotation, neckRnd2.LocalRot)) < 0.01f)
            {
                neckRnd2.IsRotStart = false;
                neckRnd3.IsRotStart = true;
            }
            if (neckRnd3.IsRotStart && !neckRnd3.IsGenerate)
            {
                neckRnd3.IsGenerate = true;
                neckRnd3.NeckBoneParentRotation = neckBone.parent.rotation;
                neckRnd3.TargetRotGenerate();
                neckRnd3.IsRotStart = true;
            }
            if (neckRnd3.IsRotStart)
            {
                neckOffset.rotation =
                    Quaternion.Slerp(
                        neckOffset.rotation,
                        neckRnd3.TargetRot,
                        15f * Time.deltaTime);
            }
        }
        else
        {
            if (statusPlayer.PlayerStatus == PlayerStatus.Fine &&
                !isAimState) neckOffset.localRotation = Quaternion.identity;

            if (animator.GetFloat("moveY") == 1f)
            {
                hipsOffset.localRotation =
                        Quaternion.Slerp(
                            hipsOffset.localRotation,
                            runHipsRot,
                            runHipsSlerpSpeed * Time.deltaTime);
            }
            if (animator.GetFloat("moveX") == 0f && animator.GetFloat("moveY") == 0f)
            {
                hipsOffset.localRotation =
                        Quaternion.Slerp(
                            hipsOffset.localRotation,
                            defaultHipsRot,
                            runHipsSlerpSpeed * Time.deltaTime);
            }
            if (animator.GetFloat("moveY") == -1f)
            {
                hipsOffset.localRotation =
                        Quaternion.Slerp(
                            hipsOffset.localRotation,
                            runBackHipsRot,
                            runHipsSlerpSpeed * Time.deltaTime);
            }
            if (animator.GetFloat("moveX") == -1f || animator.GetFloat("moveX") == 1f)
            {
                Vector3 tmp = hipsOffset.localPosition;
                tmp.y += leftRightHipsOffsetY;
                hipsOffset.localPosition = tmp;
            }
        }

        if (statusPlayer.PlayerStatus != PlayerStatus.Dead &&
            statusPlayer.PlayerStatus != PlayerStatus.FallDown &&
            statusPlayer.PlayerStatus != PlayerStatus.Recover &&
            groundMoter.gravityDamageInfo.DamageType != DamageType.Falling)
        {
            IkFoot(left);
            IkFoot(right);
        }

        void IkFoot(Data data)
        {
            data.kneeHint.position = data.upLeg.position +
                transform.forward * 0.4f +
                transform.right * data.kneeOffset +
                transform.up * data.kneeHeight;

            Vector3 origin = data.originFoot.position;
            origin.y += rayOffset;

            Log.R(origin, Vector3.down * rayLength, Color.blue);

            bool check = Physics.Raycast(
                origin, Vector3.down, out RaycastHit hit, rayLength, groundLayer
                );
            if (check) data.hitFoot = hit.point;
            else data.hitFoot = transform.position;

            if (!data.footLock && check && data.animatorFootIkValue == 1f)
            {
                data.footLock = true;
                data.footIk.weight = 1f;
                soundPlayer.FootSteps(
                    groundManager.GetGround(hit.collider.gameObject).SurfaceType);
            }
            if (data.footLock && data.animatorFootIkValue == 0f)
            {
                data.footLock = false;
                data.footIk.weight = 0f;
            }
            if (data.footLock)
            {
                Vector3 tmp = data.hitFoot;
                float curY = data.footIkTarget.position.y;
                tmp.y -= graphics_localPosition_y;
                tmp.y = Mathf.Lerp(
                    curY,
                    tmp.y,
                    lerpSpeed * Time.deltaTime);
                data.footIkTarget.position = tmp;

                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                Quaternion rot = Quaternion.LookRotation(forward, hit.normal);
                data.footIkTarget.rotation = rot * data.footAxisOffset;
            }
        }

        if (DeadType != DeadType.DropOut &&
            (statusPlayer.PlayerStatus == PlayerStatus.Dead ||
            statusPlayer.PlayerStatus == PlayerStatus.FallDown &&
            groundMoter.gravityDamageInfo.ShockPower > 200))
        {
            IkArmFallDown(left);
            IkArmFallDown(right);

            Vector3 localAngle = new Vector3(-135f, 45f, 60f);
            left.armIkTarget.localRotation = Quaternion.Euler(localAngle);            
            localAngle = new Vector3(150f, 100f, 80f);
            right.armIkTarget.localRotation = Quaternion.Euler(localAngle);
        }
        else
        {
            for (int i = 0; i < armRnd_leftright.Length; i++)
            {
                if (armRnd_leftright[i].IsGenerate)
                {
                    armRnd_leftright[i].IsGenerate = false;
                }
            }
        }

        void IkArmFallDown(Data data)
        {
            if (data.originArmFallDown.localPosition.x < 0f)
            {
                if (!armRnd_leftright[0].IsGenerate)
                {
                    armRnd_leftright[0].IsGenerate = true;
                    data.originArmFallDown.localPosition = new Vector3(-0.44f, 0f, -0.15f);
                    armRnd_leftright[0].OriginPos = data.originArmFallDown.localPosition;
                    armRnd_leftright[0].TargetPosGenerate();
                    data.originArmFallDown.localPosition = armRnd_leftright[0].TargetPos;
                }
            }
            else
            {
                if (!armRnd_leftright[1].IsGenerate)
                {
                    armRnd_leftright[1].IsGenerate = true;
                    data.originArmFallDown.localPosition = new Vector3(0.44f, 0f, -0.15f);
                    armRnd_leftright[1].OriginPos = data.originArmFallDown.localPosition;
                    armRnd_leftright[1].TargetPosGenerate();
                    data.originArmFallDown.localPosition = armRnd_leftright[1].TargetPos;
                }
            }
            
            Vector3 origin = data.originArmFallDown.position;

            origin.y += rayOffset;

            Log.R(origin, Vector3.down * rayLength_IkArmFallDown, new Color(255f, 0f, 255f));

            bool check = Physics.Raycast(
                origin, Vector3.down, out RaycastHit hit, rayLength, groundLayer
                );
            Log.D($"IkArmFallDown Hit: {check}");
            if (check) data.hitArmFallDown = hit.point;
            else data.hitArmFallDown = transform.position;

            if (!data.armFallDownLock && check)
            {
                data.armFallDownLock = true;
                data.armIk.weight = 1f;
                Vector3 tmp = data.elbowHint.localPosition;
                tmp.y = -1.2f;
                data.elbowHint.localPosition = tmp;
            }
            if (data.armFallDownLock && !check)
            {
                data.armFallDownLock = false;
                data.armIk.weight = 1f;
            }
            if (data.armFallDownLock)
            {
                Vector3 tmp = data.hitArmFallDown;
                tmp.y -= graphics_localPosition_y;

                if (Mathf.Abs(data.armIkTarget.position.x - tmp.x) < 0.02f &&
                    Mathf.Abs(data.armIkTarget.position.y - tmp.y) < 0.02f &&
                    Mathf.Abs(data.armIkTarget.position.z - tmp.z) < 0.02f)
                {
                    data.armIkTarget.position = tmp;
                }
                else
                {
                    data.armIkTarget.position =
                        Vector3.Lerp(
                        data.armIkTarget.position,
                        tmp,
                        2f * lerpSpeed * Time.deltaTime);
                }
            }
            else
            {
                Vector3 tmp = data.originArmFallDown.position;
                tmp.y -= 1f;

                if (Mathf.Abs(data.armIkTarget.position.x - tmp.x) < 0.02f &&
                    Mathf.Abs(data.armIkTarget.position.y - tmp.y) < 0.02f &&
                    Mathf.Abs(data.armIkTarget.position.z - tmp.z) < 0.02f)
                {
                    data.armIkTarget.position = tmp;
                }
                else
                {
                    data.armIkTarget.position =
                        Vector3.Lerp(
                        data.armIkTarget.position,
                        tmp,
                        lerpSpeed * Time.deltaTime);
                }
                Vector3 tmpElbow = data.elbowHint.localPosition;
                tmpElbow.y = -1.2f;
                data.elbowHint.localPosition = tmpElbow;
            }
        }

        if (DeadType != DeadType.DropOut &&
            (statusPlayer.PlayerStatus == PlayerStatus.Dead ||
            statusPlayer.PlayerStatus == PlayerStatus.FallDown &&
            groundMoter.gravityDamageInfo.ShockPower > 200))
        {
            IkFootFallDown(left);
            IkFootFallDown(right);

            Vector3 localAngle = left.footIkTarget.localEulerAngles;
            localAngle.x = 90f;
            left.footIkTarget.localRotation = Quaternion.Euler(localAngle);
            localAngle = right.footIkTarget.localEulerAngles;
            localAngle.x = 90f;
            right.footIkTarget.localRotation = Quaternion.Euler(localAngle);
        }
        else
        {
            for (int i = 0; i < footRnd_leftright.Length; i++)
            {
                if (footRnd_leftright[i].IsGenerate)
                {
                    footRnd_leftright[i].IsGenerate = false;
                }
            }
        }

        void IkFootFallDown(Data data)
        {
            if (data.originFootFallDown.localPosition.x < 0f)
            {
                if (!footRnd_leftright[0].IsGenerate)
                {
                    footRnd_leftright[0].IsGenerate = true;
                    data.originFootFallDown.localPosition = new Vector3(-0.1f, 0f, 0.675f);
                    footRnd_leftright[0].OriginPos = data.originFootFallDown.localPosition;
                    footRnd_leftright[0].TargetPosGenerate();
                    data.originFootFallDown.localPosition = footRnd_leftright[0].TargetPos;
                }
            }
            else
            {
                if (!footRnd_leftright[1].IsGenerate)
                {
                    footRnd_leftright[1].IsGenerate = true;
                    data.originFootFallDown.localPosition = new Vector3(0.1f, 0f, 0.675f);
                    footRnd_leftright[1].OriginPos = data.originFootFallDown.localPosition;
                    footRnd_leftright[1].TargetPosGenerate();
                    data.originFootFallDown.localPosition = footRnd_leftright[1].TargetPos;
                }
            }

            Vector3 origin = data.originFootFallDown.position;
            origin.y += rayOffset;

            Log.R(origin, Vector3.down * rayLength_IkArmFallDown, new Color(0f, 255f, 255f));

            bool check = Physics.Raycast(
                origin, Vector3.down, out RaycastHit hit, rayLength, groundLayer
                );
            Log.D($"IkFootFallDown Hit: {check}");
            if (check) data.hitFootFallDown = hit.point;
            else data.hitFootFallDown = transform.position;

            if (!data.footFallDownLock && check)
            {
                data.footFallDownLock = true;
                data.footIk.weight = 1f;
                Log.D($"1f: {data.footIk.weight}");

                Log.D($"HIT: {data.footFallDownLock}");
                Vector3 tmp = data.kneeHint.localPosition;
                tmp.y = 2.5f;
                data.kneeHint.localPosition = tmp;
            }
            if (data.footFallDownLock && !check)
            {
                data.footFallDownLock = false;
                data.footIk.weight = 1f;
            }
            if (data.footFallDownLock)
            {
                Vector3 tmp = data.hitFootFallDown;
                tmp.y -= graphics_localPosition_y;

                if (Mathf.Abs(data.footIkTarget.position.x - tmp.x) < 0.02f &&
                    Mathf.Abs(data.footIkTarget.position.y - tmp.y) < 0.02f &&
                    Mathf.Abs(data.footIkTarget.position.z - tmp.z) < 0.02f)
                {
                    data.footIkTarget.position = tmp;
                }
                else
                {
                    data.footIkTarget.position =
                        Vector3.Lerp(
                        data.footIkTarget.position,
                        tmp,
                        2f * lerpSpeed * Time.deltaTime);
                }
            }
            else
            {
                Vector3 tmp = data.originFootFallDown.position;
                tmp.y -= 1f;

                if (Mathf.Abs(data.footIkTarget.position.x - tmp.x) < 0.02f &&
                    Mathf.Abs(data.footIkTarget.position.y - tmp.y) < 0.02f &&
                    Mathf.Abs(data.footIkTarget.position.z - tmp.z) < 0.02f)
                {
                    data.footIkTarget.position = tmp;
                }
                else
                {
                    data.footIkTarget.position =
                        Vector3.Lerp(
                        data.footIkTarget.position,
                        tmp,
                        lerpSpeed * Time.deltaTime);
                }
            }
        }

        if (!isRecoverEnter &&
            statusPlayer.PlayerStatus == PlayerStatus.Recover &&
            groundMoter.IsGrounded)
        {
            isRecoverEnter = true;
            animator.SetBool("isAiming", false);
            animator.SetBool("isAimFinished", true);
            ArmIkOff();
            FootIkOff();
            hipsConstraint.weight = 0f;
            animator.SetTrigger("recover");
            statusPlayer.RaiseDamage(PlayerStatus.Recover, 0);
        }

        if (isRecoverExit)
        {
            isRecoverExit = false;
            hipsConstraint.weight = 1f;
            hipsOffset.localRotation = Quaternion.identity;
            isRecoverLerp = true;
            hipsOffset.localPosition = new Vector3(0f, -0.25f, 0f);
        }

        if (isRecoverLerp)
        {
            hipsOffset.localPosition =
                Vector3.Lerp(
                    hipsOffset.localPosition,
                    new Vector3(0f, -0.2f, 0f),
                    5f * Time.deltaTime);
            if (Mathf.Abs(
                hipsOffset.localPosition.magnitude - new Vector3(0f, -0.2f, 0f).magnitude) < 0.02f)
            {
                isRecoverLerp = false;
            }
        }

        if (isRecoverIkCancel)
        {
            isRecoverIkCancel = false;
            hipsConstraint.weight = 1f;
        }
    }

    public void OnAimStateEnter()
    {
        isAimState = true;
        inputManager.deltaY = 0f;
        line.gameObject.SetActive(true);
    }
    public void OnAimStateExit()
    {
        isAimState = false;
        isDeltaY_init = false;
        line.gameObject.SetActive(false);
    }
    public void OnRecoverStateExit()
    {
        if (statusPlayer.PlayerStatus != PlayerStatus.Dead)
        {
            groundMoter.gravityDamageInfo.DamageType = DamageType.None;
        }
        isRecoverEnter = false;
        isRecoverIkCancel = true;
        neckRnd2.IsGenerate = false;
        neckRnd3.IsGenerate = false;
        neckRnd2.IsRotStart = false;
        neckRnd3.IsRotStart = false;
        neckOffset.localRotation = Quaternion.identity;
    }
    public void OnDamageStateExit()
    {
        groundMoter.gravityDamageInfo.DamageType = DamageType.None;
        if (statusPlayer.PlayerStatus != PlayerStatus.Dead)
        {
            statusPlayer.PlayerStatus = PlayerStatus.Fine;
        }
        groundMoter.gravityDamageInfo.CanAnimOverride = true;
        groundMoter.gravityDamageInfo.CurShockPower = 0;
    }
    public void OnRecoverAnimExit()
    {
        groundMoter.gravityDamageInfo.CurShockPower = 0;
        isRecoverExit = true;
        if (statusPlayer.PlayerStatus != PlayerStatus.Dead)
        {
            statusPlayer.PlayerStatus = PlayerStatus.Fine;
        }
    }
    public void OnDamage3AnimExit()
    {
        if (statusPlayer.PlayerStatus != PlayerStatus.Dead)
        {
            statusPlayer.PlayerStatus = PlayerStatus.Recover;
        }
    }
    public void ArmIkOn()
    {
        left.armIk.weight = 1f;
        right.armIk.weight = 1f;
    }
    public void ArmIkOff()
    {
        left.armIk.weight = 0f;
        right.armIk.weight = 0f;
    }
    public void FootIkOn()
    {
        left.footIk.weight = 1f;
        right.footIk.weight = 1f;
    }
    public void FootIkOff()
    {
        left.footIk.weight = 0f;
        right.footIk.weight = 0f;
    }
}
