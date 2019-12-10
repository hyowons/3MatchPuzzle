using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroTest : MonoBehaviour {


    public GameObject patty;

    public GameObject pad;

    public UILabel accelatorLabel;

    public UIButton TouchButton;
    // Update is called once per frame

    public float accelate = 20f;

    Rigidbody meatRigid = null;

    private void Awake()
    {
        meatRigid = patty.GetComponent<Rigidbody>();
        meatRigid.maxAngularVelocity = 10f;
        meatRigid.AddTorque(Vector3.up * 0.5f);


        TouchButton.onClick.Add(new EventDelegate(ResetPatty));

    }

    float _smashPoint = 0f;
    void Update() {
        //Input.acceleration	

        _testAcceleartion = Vector3.zero;
        if (Input.GetKey(KeyCode.DownArrow))
            _testAcceleartion += Vector3.down;
        //pad.transform.eulerAngles = pad.transform.eulerAngles + (Vector3.left * (Time.deltaTime * accelate));

        if (Input.GetKey(KeyCode.UpArrow))
            _testAcceleartion += Vector3.up;
        //pad.transform.eulerAngles = pad.transform.eulerAngles + (Vector3.right * (Time.deltaTime * accelate));

        if (Input.GetKey(KeyCode.LeftArrow))
            _testAcceleartion += Vector3.left;
        //pad.transform.eulerAngles = pad.transform.eulerAngles + (Vector3.forward * (Time.deltaTime * accelate));

        if (Input.GetKey(KeyCode.RightArrow))
            _testAcceleartion += Vector3.right;
        //pad.transform.eulerAngles = pad.transform.eulerAngles + (Vector3.back * (Time.deltaTime * accelate));

        _testAcceleartion.Normalize();

        if (Input.GetKey(KeyCode.Space))
        {
            _smashPoint += (Time.deltaTime * 500f);
            Debug.Log(_smashPoint);
        }
        

        if (Input.GetKeyUp(KeyCode.Space))
            smash();

        if (Input.GetKeyDown(KeyCode.Return))
            meatRigid.WakeUp();

    }

    Vector3 _lerpAcceleartion = Vector3.zero;
    Vector3 _curAcceleartion = Vector3.zero;
    Vector3 _testAcceleartion = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 curAcc = Vector3.Lerp(_curAcceleartion, _lerpAcceleartion, Time.deltaTime * 20f);
        _curAcceleartion = curAcc;

        Vector3 acc = Vector3.zero;
        //acc.x = Mathf.Clamp(curAcc.y * 20f, -10, 30);
        acc.x = Mathf.Clamp(curAcc.y * 70f, -10, 30);
        acc.z = Mathf.Clamp(-curAcc.x * 20f, -5, 5);
        acc.y = 0f;

        pad.transform.eulerAngles = acc;

        meatRigid.WakeUp();
        //accelatorLabel.text = _curAcceleartion.x.ToString() + ", " + _curAcceleartion.y.ToString() + ", " + _curAcceleartion.z.ToString();
        accelatorLabel.text = meatRigid.velocity.x.ToString() + ", " + meatRigid.velocity.y.ToString() + ", " + meatRigid.velocity.z.ToString();

#if !UNITY_EDITOR
        _lerpAcceleartion = Input.acceleration;
#else
        _lerpAcceleartion = _testAcceleartion;
#endif

    }

    void smash()
    {
        Debug.LogError("Smash");
        meatRigid.AddForce(Vector3.up * _smashPoint, ForceMode.Force);
        meatRigid.AddTorque(Vector3.right * _smashPoint, ForceMode.Force);
        _smashPoint = 0f;
    }


    private void ResetPatty()
    {

        patty.transform.localPosition = new Vector3(0, 3f, 0);
        var angle = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        patty.transform.eulerAngles = angle;
        //meatRigid.AddForce(Vector3.up);
        meatRigid.ResetInertiaTensor();
        //meatRigid.Sleep();
        meatRigid.velocity = Vector3.zero;
        meatRigid.AddTorque(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        
    }



    //private void OnGUI()
    //{
    //    if (GUILayout.Button("Reset") == true)
    //    {
    //        ResetPatty();
    //    }
    //}
}
