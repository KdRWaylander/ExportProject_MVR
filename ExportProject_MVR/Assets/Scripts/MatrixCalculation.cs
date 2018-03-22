using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixCalculation : MonoBehaviour {
    public enum ComputationMethod { Euler, Quaternion };

    Matrix4x4 m_rotationMatrix;

    void Start()
    {
        m_rotationMatrix = Matrix4x4.identity;
    }

    /* PUBLIC METHODS */
    public Vector3 ComputeResult(RaycastHit _hit, ComputationMethod _computationMethod)
    {
        Vector3 targetSize = GetWorldSpaceTargetSize(_hit);
        Vector3 unscaledResultWorld = GetWorldSpaceVector(_hit);
        Vector3 unscaledResultLocal;

        if (_computationMethod == ComputationMethod.Euler)
        {
            unscaledResultLocal = GetLocalSpaceVector_Euler(unscaledResultWorld, _hit.transform.parent.rotation);
        }
        else if (_computationMethod == ComputationMethod.Quaternion)
        {
            unscaledResultLocal = GetLocalSpaceVector_Quaternion(unscaledResultWorld, _hit.transform.parent.rotation);
        }
        else
        {
            unscaledResultLocal = Vector3.zero;
        }

        Vector3 relativeError = new Vector3(unscaledResultLocal.x / (targetSize.x * 0.5f), unscaledResultLocal.y / (targetSize.z * 0.5f), unscaledResultLocal.z / (targetSize.x * 0.5f));
        Debug.Log(relativeError.ToString());
        return relativeError;
    }

    /* PRIVATE METHODS */
    private Vector3 GetWorldSpaceVector(RaycastHit _hit)
    {
        Vector3 hitCenterPosition, hitPointCoordinates;
        hitCenterPosition = _hit.transform.parent.position;
        hitPointCoordinates = _hit.point;

        Vector3 resultVector;
        resultVector.x = Mathf.Abs(hitPointCoordinates.x - hitCenterPosition.x);
        resultVector.y = Mathf.Abs(hitPointCoordinates.y - hitCenterPosition.y);
        resultVector.z = Mathf.Abs(hitPointCoordinates.z - hitCenterPosition.z);

        return resultVector;
    }

    private Vector3 GetWorldSpaceTargetSize(RaycastHit _hit)
    {
        Vector3 size = _hit.transform.gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
        Vector3 scale = _hit.transform.lossyScale;
        Vector3 scaledSize = new Vector3(size.x * scale.x, size.y * scale.y, size.z * scale.z);
        return scaledSize;
    }

    private Vector3 GetLocalSpaceVector_Euler(Vector3 _worldBaseVector, Quaternion _localRotation)
    {
        Vector3 wBV = _worldBaseVector;                                    // Hit point coordinates (former vector in world base)
        float rotX = _localRotation.eulerAngles.x * 2 * Mathf.PI / 360f;   // Degrees -> radians
        float rotY = _localRotation.eulerAngles.y * 2 * Mathf.PI / 360f;   // Degrees -> radians
        float rotZ = _localRotation.eulerAngles.z * 2 * Mathf.PI / 360f;   // Degrees -> radians
        float a11, a12, a13, a21, a22, a23, a31, a32, a33;                 // Matrix coefficients

        // Rotation matrix coefficients explicited for general case
        a11 = Mathf.Cos(rotZ) * Mathf.Cos(rotY);
        a12 = Mathf.Cos(rotZ) * Mathf.Sin(rotY) * Mathf.Sin(rotX) - Mathf.Sin(rotZ) * Mathf.Cos(rotX);
        a13 = Mathf.Cos(rotZ) * Mathf.Sin(rotY) * Mathf.Cos(rotX) + Mathf.Sin(rotZ) * Mathf.Sin(rotX);

        a21 = Mathf.Sin(rotZ) * Mathf.Cos(rotY) - Mathf.Sin(rotY);
        a22 = Mathf.Sin(rotX) * (Mathf.Sin(rotZ) * Mathf.Sin(rotY) + Mathf.Cos(rotY)) + Mathf.Cos(rotZ) * Mathf.Cos(rotX);
        a23 = Mathf.Cos(rotX) * (Mathf.Sin(rotZ) * Mathf.Sin(rotY) + Mathf.Cos(rotY)) - Mathf.Cos(rotZ) * Mathf.Sin(rotX);

        a31 = -Mathf.Sin(rotY);
        a32 = Mathf.Sin(rotX) * Mathf.Cos(rotY);
        a33 = Mathf.Cos(rotX) * Mathf.Cos(rotY);

        // New vector in local base
        Vector3 rotatedVector;
        rotatedVector.x = a11 * wBV.x + a12 * wBV.y + a13 * wBV.z;
        rotatedVector.y = a21 * wBV.x + a22 * wBV.y + a23 * wBV.z;
        rotatedVector.z = a31 * wBV.x + a32 * wBV.y + a33 * wBV.z;

        return rotatedVector;
    }

    private Vector3 GetLocalSpaceVector_Quaternion(Vector3 _worldBaseVector, Quaternion _localRotation)
    {
        Vector3 wBV = _worldBaseVector;                                    // Hit point coordinates (former vector in world base)
        float w, x, y, z;                                                  // Shortcuts for quaternion members
        float a11, a12, a13, a21, a22, a23, a31, a32, a33;                 // Matrix coefficients

        // Quaternion members
        w = _localRotation.w;
        x = _localRotation.x;
        y = _localRotation.y;
        z = _localRotation.z;

        // Normalization https://stackoverflow.com/questions/1556260/convert-quaternion-rotation-to-rotation-matrix
        float n = 1f / Mathf.Sqrt(Mathf.Pow(w, 2) + Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
        w *= n; x *= n; y *= n; z *= n;

        // Rotation matrix coefficients explicited for general case
        // https://en.wikipedia.org/wiki/Rotation_matrix#Quaternion
        a11 = 1 - 2 * Mathf.Pow(y, 2) - 2 * Mathf.Pow(z, 2); m_rotationMatrix[0, 0] = a11;
        a12 = 2 * (x * y - z * w); m_rotationMatrix[0, 1] = a12;
        a13 = 2 * (x * z + y * w); m_rotationMatrix[0, 2] = a13;
        m_rotationMatrix[0, 3] = 0;

        a21 = 2 * (x * y + z * w); m_rotationMatrix[1, 0] = a21;
        a22 = 1 - 2 * Mathf.Pow(x, 2) - 2 * Mathf.Pow(z, 2); m_rotationMatrix[1, 1] = a22;
        a23 = 2 * (y * z - x * w); m_rotationMatrix[1, 2] = a23;
        m_rotationMatrix[1, 3] = 0;

        a31 = 2 * (x * z - y * w); m_rotationMatrix[2, 0] = a31;
        a32 = 2 * (y * z + x * w); m_rotationMatrix[2, 1] = a31;
        a33 = 1 - 2 * Mathf.Pow(x, 2) - 2 * Mathf.Pow(y, 2); m_rotationMatrix[2, 2] = a31;
        m_rotationMatrix[2, 3] = 0;

        m_rotationMatrix[3, 0] = 0;
        m_rotationMatrix[3, 1] = 0;
        m_rotationMatrix[3, 2] = 0;
        m_rotationMatrix[3, 3] = 1;
        // Matrix inversion
        Matrix4x4 m_inverseRotationMatrix = m_rotationMatrix.inverse; m_inverseRotationMatrix = m_inverseRotationMatrix.transpose;
        Debug.Log(m_inverseRotationMatrix.ToString());

        // New vector in local base
        // Vb = Q Va Q-1
        Vector3 halfRotatedVector;                                          // First multiplication
        halfRotatedVector.x = a11 * wBV.x + a12 * wBV.y + a13 * wBV.z;
        halfRotatedVector.y = a21 * wBV.x + a22 * wBV.y + a23 * wBV.z;
        halfRotatedVector.z = a31 * wBV.x + a32 * wBV.y + a33 * wBV.z;

        Vector3 rotatedVector;                                              // Second multiplication
        rotatedVector.x = m_rotationMatrix[0, 0] * halfRotatedVector.x + m_rotationMatrix[0, 1] * halfRotatedVector.y + m_rotationMatrix[0, 2] * halfRotatedVector.z;
        rotatedVector.y = m_rotationMatrix[1, 0] * halfRotatedVector.x + m_rotationMatrix[1, 1] * halfRotatedVector.y + m_rotationMatrix[1, 2] * halfRotatedVector.z;
        rotatedVector.z = m_rotationMatrix[2, 0] * halfRotatedVector.x + m_rotationMatrix[2, 1] * halfRotatedVector.y + m_rotationMatrix[2, 2] * halfRotatedVector.z;

        return rotatedVector;
    }
}