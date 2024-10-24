using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;
    public MovementInput moveScript;
    public float speedBoost = 6;
    public Animator animator;
    public float animSpeedBoost = 1.5f;

    [Header("Mesh Releted")]
    public float meshRefereshRate = 0.1f;
    public float meshDestroyDelay = 3.0f;
    public Transform positionToSpawn;

    [Header("Shader Releted")]
    public Material mat;
    public string shadervarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private SkinnedMeshRenderer[] SkinnedRenderer;
    private bool isTrailactive;

    private float normalSpeed;
    private float normalaAnimSpeed;

    IEnumerator AnimateMaterialFloat(Material m, float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shadervarRef);


        while (valueToAnimate > rate)
        {
            valueToAnimate -= rate;
            m.SetFloat(shadervarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
    IEnumerator ActivateTrail(float timeActivated)
    {
        normalSpeed = moveScript.movementSpeed;
        moveScript.movementSpeed = speedBoost;

        normalaAnimSpeed = animator.GetFloat("animSpeed");
        animator.SetFloat("animSpeed",animSpeedBoost);

        while (timeActivated > 0)
        {
            timeActivated -= meshRefereshRate;

            if (SkinnedRenderer == null)
                SkinnedRenderer = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < SkinnedRenderer.Length; i++)
            {
                GameObject g0bj = new GameObject();
                g0bj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = g0bj.AddComponent<MeshRenderer>();
                MeshFilter mf = g0bj.AddComponent<MeshFilter>();

                Mesh m = new Mesh();
                SkinnedRenderer[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = mat;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(g0bj, meshDestroyDelay);

            }
            yield return new WaitForSeconds(meshRefereshRate);
        }
        moveScript.movementSpeed = normalSpeed;
        animator.SetFloat("animSpeed", normalaAnimSpeed);
        isTrailactive = false;
    }
        

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) && !isTrailactive)
            {
                isTrailactive=true;
                StartCoroutine(ActivateTrail(activeTime));
            }
        }
    }
