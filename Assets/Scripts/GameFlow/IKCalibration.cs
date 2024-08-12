using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using TMPro;

public class IKCalibration : MonoBehaviour
{
    private VRIK m_VRIK;
    //private SteamVR_Controller.Device m_controller_1, m_controller_2;

    private bool m_userCalibrated = false;

    private GameObject m_headIKTarget;
    private GameObject m_leftHandIKTarget;
    private GameObject m_rightHandIKTarget;

    public Transform headTracker;
    public Transform leftHandTracker;
    public Transform rightHandTracker;

    public List<Transform> otherTrackers;

    private Transform m_pelvisTracker;
    private Transform m_leftToesTracker;
    private Transform m_RightToesTracker;

    private bool m_enableCalibrationProcess;
    private bool calibDone = false;

    private void endCalib()
    {
        Debug.LogWarning("END Avatar done");
        calibDone = false;
    }

    public bool UserCalibrated
    {
        get { return m_userCalibrated; }
    }

    void Start()
    {
        m_VRIK = transform.GetComponent<VRIK>();
        m_VRIK.solver.IKPositionWeight = 0;

        /*try
        {
        //    m_controller_1 = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));
        //    m_controller_2 = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Please connect the Vive controllers " + e.ToString());
        }*/

        headTracker.GetChild(0).gameObject.SetActive(true);
        leftHandTracker.GetChild(0).gameObject.SetActive(true);
        rightHandTracker.GetChild(0).gameObject.SetActive(true);

        foreach (Transform tracker in otherTrackers)
            tracker.GetChild(0).gameObject.SetActive(true);
    }

    void Update()
    {
        if (!m_userCalibrated)
        {
            SetAvatarHeight();
            SetIKTrackingPoints();
        }
        else if (calibDone)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                endCalib();
            }
        }
    }

    public void SetAvatarHeight()
    {
        float actualEyeHeight = Camera.main.transform.localPosition.y;

        actualEyeHeight = Mathf.Clamp(actualEyeHeight, 0.7f, 2.5f);

        float scaleFactor = 0.6f;
        float avatarScale = actualEyeHeight * scaleFactor;
        transform.localScale = new Vector3(avatarScale, avatarScale, avatarScale);
    }

    public void SetIKTrackingPoints()
    {
        m_pelvisTracker = otherTrackers[0];
        int pelvisTrackerID = 0;

        for (int i = 1; i < otherTrackers.Count; i++)
        {
            if (otherTrackers[i].position.y > m_pelvisTracker.position.y)
            {
                m_pelvisTracker = otherTrackers[i];
                pelvisTrackerID = i;
            }
        }

        otherTrackers.Remove(otherTrackers[pelvisTrackerID]);

        if (otherTrackers.Count == 2)
        {
            if (otherTrackers[0].position.x > otherTrackers[1].position.x)
            {
                m_RightToesTracker = otherTrackers[0];
                m_leftToesTracker = otherTrackers[1];
            }
            else
            {
                m_RightToesTracker = otherTrackers[1];
                m_leftToesTracker = otherTrackers[0];
            }
        }

        else
            Debug.Log("Incorrect number of trackers");

        m_headIKTarget = Instantiate(Resources.Load("IKTarget", typeof(GameObject))) as GameObject;
        m_headIKTarget.transform.position = m_VRIK.references.head.transform.position;
        m_headIKTarget.transform.localRotation = m_VRIK.references.head.transform.rotation;
        m_headIKTarget.transform.parent = headTracker;
        headTracker.GetChild(0).gameObject.SetActive(false);
        m_VRIK.solver.spine.headTarget = m_headIKTarget.transform;

        m_leftHandIKTarget = Instantiate(Resources.Load("IKTarget", typeof(GameObject))) as GameObject;
        m_leftHandIKTarget.transform.position = m_VRIK.references.leftHand.transform.position;
        m_leftHandIKTarget.transform.localRotation = m_VRIK.references.leftHand.transform.rotation;
        m_leftHandIKTarget.transform.parent = leftHandTracker;
        leftHandTracker.GetChild(0).gameObject.SetActive(false);
        m_VRIK.solver.leftArm.target = m_leftHandIKTarget.transform;

        m_rightHandIKTarget = Instantiate(Resources.Load("IKTarget", typeof(GameObject))) as GameObject;
        m_rightHandIKTarget.transform.position = m_VRIK.references.rightHand.transform.position;
        m_rightHandIKTarget.transform.rotation = m_VRIK.references.rightHand.transform.rotation;
        m_rightHandIKTarget.transform.parent = rightHandTracker;
        rightHandTracker.GetChild(0).gameObject.SetActive(false);
        m_VRIK.solver.rightArm.target = m_rightHandIKTarget.transform;

        m_VRIK.solver.IKPositionWeight = 1;

        m_userCalibrated = true;
        calibDone = true;
    }
}
