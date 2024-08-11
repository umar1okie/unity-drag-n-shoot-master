using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float selectAnimTime;
    [SerializeField] private float deselectAnimTime;

    [Header("Drag Settings")]
    [SerializeField] private float amplitude;

    [Header("Spring")]
    [SerializeField] private float stiffness;
    [SerializeField] float clamp;

    [Header("Points")]
    [SerializeField] private List<GameObject> points = new List<GameObject>();
    [SerializeField] private List<GameObject> launchPoints = new List<GameObject>();
    private List<Vector3> pointStartPos = new List<Vector3>();
    private GameObject selectedObject;

    private void Awake()
    {
        foreach (GameObject point in points)
        {
            pointStartPos.Add(point.transform.localPosition);
        }
    }

    private void Update()
    {
        if (selectedObject != null)
        {
            StopAllCoroutines();
            Selected(selectedObject);
        }
    }

    private void HandleRotation(GameObject item)
    {
        Vector2 dir = GetInputPosition() - (Vector2)Camera.main.WorldToScreenPoint(this.transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        item.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void HandlePoint(Vector3 inputPosRelative, float distance, int i, Point point)
    {
        int dir = point.flip ? -1 : 1;
        Vector3 startPos = pointStartPos[i];
        float magnitude = (amplitude * distance) / distance;
        float pointIdentity = (startPos.x * magnitude) * dir;
        Vector3 targetPos = (inputPosRelative * pointIdentity) * dir;
        float lerpTime = (selectAnimTime / pointIdentity) * Time.deltaTime;
        Vector3 lerpPos = Vector3.Lerp(point.transform.localPosition, targetPos, lerpTime);
        lerpPos.z = 0;
        float pointDistance = Vector3.Distance(point.transform.position, this.transform.position);
        lerpPos = Vector3.ClampMagnitude(lerpPos, (pointIdentity / magnitude) + (pointDistance / clamp));
        point.transform.localPosition = lerpPos;
    }

    public void Selected(GameObject selected)
    {
        this.gameObject.SetActive(true);
        Transform cannonHole = selected.GetComponent<Clickable>().GetCannonHole();
        this.transform.position = cannonHole.position; // Align reticle with cannon hole
        selectedObject = selected;

        Vector3 inputPos = GetInputPosition();
        Vector3 inputWorldPos = Camera.main.ScreenToWorldPoint(inputPos);
        Vector3 inputPosRelative = inputWorldPos - this.transform.position;

        for (int i = 0; i < points.Count; i++)
        {
            HandleRotation(points[i].gameObject);
            float distance = Vector2.Distance(this.transform.position, inputWorldPos);
            Point point = points[i].GetComponent<Point>();
            HandlePoint(inputPosRelative, distance, i, point);
        }
    }

    public void Deselect()
    {
        selectedObject = null;
        if (gameObject.activeInHierarchy)
        {
            for (int i = 0; i < points.Count; i++)
            {
                StartCoroutine(LerpObject(points[i], Vector3.zero, deselectAnimTime));
            }
        }
    }

    private void FireProjectiles()
    {
        for (int i = 0; i < launchPoints.Count; i++)
        {
            launchPoints[i].GetComponent<ShootProjectile>().FireProjectile();
        }
        this.gameObject.SetActive(false);
    }


    private int count = 0;
    private IEnumerator LerpObject(GameObject item, Vector3 pos, float time)
    {
        Vector3 currentPos = item.transform.localPosition;
        float elapsed = 0f;
        float distance = Vector3.Distance(currentPos, pos);
        float ratio = 0;
        while (ratio < 1)
        {
            elapsed += Time.fixedDeltaTime;
            float offset = animCurve.Evaluate(ratio);
            float newOffset = offset - ratio;
            newOffset = newOffset / stiffness;
            offset = newOffset + ratio;
            float invertOffset = 1.0f - offset;
            item.transform.localPosition = Vector3.Lerp(currentPos, pos, ratio) * invertOffset;
            yield return null;
            ratio = (elapsed / time);
        }
        count++;
        if (count >= points.Count)
        {
            FireProjectiles();
        }
    }

    private Vector2 GetInputPosition()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }
        if (Input.GetMouseButton(0))
        {
            return Input.mousePosition;
        }
        return Vector2.zero;
    }
}
