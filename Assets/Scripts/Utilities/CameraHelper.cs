using UnityEngine;
using System.Reflection;

/// <summary>
/// CameraHelper - Hỗ trợ snap camera ngay lập tức khi teleport
/// Giải quyết vấn đề camera lerp từ vị trí cũ đến vị trí mới
/// Hoạt động với cả Cinemachine và camera thường
/// </summary>
public static class CameraHelper
{
    /// <summary>
    /// Thông báo cho Cinemachine rằng target đã teleport, camera cần snap ngay
    /// Gọi hàm này SAU khi di chuyển player
    /// </summary>
    public static void NotifyTargetTeleported(Transform target, Vector3 positionDelta)
    {
        if (target == null) return;

        try
        {
            var allMonoBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

            foreach (var mb in allMonoBehaviours)
            {
                if (mb == null) continue;

                var type = mb.GetType();
                var typeName = type.Name;

                // Tìm CinemachineCamera và gọi OnTargetObjectWarped
                if (typeName == "CinemachineCamera" || typeName == "CinemachineVirtualCamera")
                {
                    // Tìm method OnTargetObjectWarped với đúng signature
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var method in methods)
                    {
                        if (method.Name == "OnTargetObjectWarped")
                        {
                            var parameters = method.GetParameters();
                            if (parameters.Length == 2 &&
                                parameters[0].ParameterType == typeof(Transform) &&
                                parameters[1].ParameterType == typeof(Vector3))
                            {
                                method.Invoke(mb, new object[] { target, positionDelta });
                                Debug.Log($"[CameraHelper] Called OnTargetObjectWarped on {mb.name}");
                                break;
                            }
                        }
                    }
                }
            }

            // Di chuyển Main Camera trực tiếp
            SnapMainCamera(target);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[CameraHelper] Error in NotifyTargetTeleported: {e.Message}");
            // Fallback: snap camera trực tiếp
            SnapMainCamera(target);
        }
    }

    /// <summary>
    /// Snap camera đến vị trí target ngay lập tức
    /// </summary>
    public static void SnapCameraToTarget(Transform target)
    {
        if (target == null) return;

        try
        {
            SnapMainCamera(target);

            var allMonoBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

            foreach (var mb in allMonoBehaviours)
            {
                if (mb == null) continue;

                var type = mb.GetType();
                var typeName = type.Name;

                if (typeName == "CinemachineCamera" || typeName == "CinemachineVirtualCamera")
                {
                    // Tìm method ForceCameraPosition với đúng signature
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var method in methods)
                    {
                        if (method.Name == "ForceCameraPosition")
                        {
                            var parameters = method.GetParameters();
                            if (parameters.Length == 2)
                            {
                                Vector3 camPos = target.position + new Vector3(0, 0, -10);
                                method.Invoke(mb, new object[] { camPos, Quaternion.identity });
                                Debug.Log($"[CameraHelper] Called ForceCameraPosition on {mb.name}");
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[CameraHelper] Error in SnapCameraToTarget: {e.Message}");
        }
    }

    /// <summary>
    /// Di chuyển Main Camera trực tiếp
    /// </summary>
    private static void SnapMainCamera(Transform target)
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Vector3 newPos = target.position;
            newPos.z = mainCam.transform.position.z;
            mainCam.transform.position = newPos;
        }
    }
}

