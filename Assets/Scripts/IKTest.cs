using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Animator))]
public class IKTest : MonoBehaviour
{
    protected Animator animator;

    public VRMap head;
    public VRMap rightHand;
    public VRMap leftHand;

    public Transform headConstraint;
    public Vector3 headBodyOffset;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        headBodyOffset = transform.position - headConstraint.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.Find("Armature").position = headConstraint.position + headBodyOffset;
        transform.position = headConstraint.position + headBodyOffset;
        transform.forward = Vector3.ProjectOnPlane(headConstraint.forward, Vector3.up).normalized;


        head.Map();
        rightHand.Map();
        leftHand.Map();
    }
}
