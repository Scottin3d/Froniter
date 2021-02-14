using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public bool useScrollwheelZooming = true;
    public string zoomingAxis = "Mouse ScrollWheel";
    public bool useKeyboardZooming = true;
    public KeyCode zoomInKey = KeyCode.E;
    public KeyCode zoomOutKey = KeyCode.Q;

    #region Height

    public bool autoHeight = true;
    public LayerMask groundMask = -1; //layermask of ground or other objects that affect height

    public float maxHeight = 10f; //maximal height
    public float minHeight = 15f; //minimnal height
    public float heightDampening = 5f;
    public float keyboardZoomingSensitivity = 2f;
    public float scrollWheelZoomingSensitivity = 25f;

    private float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

    #endregion

    [SerializeField] float speed = 0.5f;
    [SerializeField] float sensitivity = 1.0f;

    Camera cam;
    Vector3 anchorPoint;
    Quaternion anchorRot;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    void FixedUpdate() {
        HeightCalculation();

        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            move += Vector3.forward * speed;
        if (Input.GetKey(KeyCode.S))
            move -= Vector3.forward * speed;
        if (Input.GetKey(KeyCode.D))
            move += Vector3.right * speed;
        if (Input.GetKey(KeyCode.A))
            move -= Vector3.right * speed;
        if (Input.GetKey(KeyCode.E))
            move += Vector3.up * speed;
        if (Input.GetKey(KeyCode.Q))
            move -= Vector3.up * speed;
        transform.Translate(move);

        if (Input.GetMouseButtonDown(1)) {
            anchorPoint = new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            anchorRot = transform.rotation;
        }
        if (Input.GetMouseButton(1)) {
            Quaternion rot = anchorRot;
            Vector3 dif = anchorPoint - new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            rot.eulerAngles += dif * sensitivity;
            transform.rotation = rot;
        }
    }

    private float ScrollWheel {
        get { return Input.GetAxis(zoomingAxis); }
    }

    private int ZoomDirection {
        get {
            bool zoomIn = Input.GetKey(zoomInKey);
            bool zoomOut = Input.GetKey(zoomOutKey);
            if (zoomIn && zoomOut)
                return 0;
            else if (!zoomIn && zoomOut)
                return 1;
            else if (zoomIn && !zoomOut)
                return -1;
            else
                return 0;
        }
    }

    private float DistanceToGround() {
        Ray ray = new Ray(cam.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, groundMask.value))
            return (hit.point - cam.transform.position).magnitude;

        return 0f;
    }

    private void HeightCalculation() {
        float distanceToGround = DistanceToGround();
        if (useScrollwheelZooming)
            zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
        if (useKeyboardZooming)
            zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;

        zoomPos = Mathf.Clamp01(zoomPos);

        float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomPos);
        float difference = 0;

        if (distanceToGround != targetHeight)
            difference = targetHeight - distanceToGround;

        cam.transform.position = Vector3.Lerp(cam.transform.position,
            new Vector3(cam.transform.position.x, targetHeight + difference, cam.transform.position.z), Time.deltaTime * heightDampening);
    }
}
