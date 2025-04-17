using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Camera minCamera;

    void Start()
    {
        transform.rotation = Quaternion.Euler(70, 0, 0); // 45도 회전
        Vector3 cameraPosition = transform.rotation * Vector3.back * 10f;
        cameraPosition.x -= 0.7f;
        transform.position = cameraPosition;
        // StartCoroutine("UpdateCamera");
        
        // 미니맵용 카메라
        if (minCamera != null)
        {
            minCamera.transform.rotation = Quaternion.Euler(90, 0, 0); // 90도 회전
            Vector3 minCameraPosition = minCamera.transform.InverseTransformPoint(new Vector3(-1f, 10f, 0f));
            minCamera.transform.position = minCameraPosition;
        }
        
        // -0.5준거는 비율 맞추기 위해서 임시처리/ 일단 x축으로 70이동시키고 중앙이 왜 안맞는지는 모르겠다
    }
    
    IEnumerator UpdateCamera()
    {
        yield return new WaitForFixedUpdate();
        // 카메라 위치 계산
        
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            var p = hit.point;
            Debug.Log($"{hit.point.ToString()} --- {hit.transform.gameObject.name}");
            transform.position -= p;
        }
        // 카메라 적용
        Debug.DrawRay(transform.position, transform.forward*100f, Color.red, 10f);
    }
}
