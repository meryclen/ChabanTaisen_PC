using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class Shape : MonoBehaviour
{
    [SerializeField] protected SkinnedMeshRenderer smr_blw_def;
    [SerializeField] protected SkinnedMeshRenderer smr_el_def;
    [SerializeField] protected SkinnedMeshRenderer smr_eye_def;
    [SerializeField] protected SkinnedMeshRenderer smr_mth_def;

    protected int blw_def_smile2_index;
    protected int blw_def_conf_index;
    protected int blw_def_ang1_index;
    protected int el_def_smile1_index;
    protected int el_def_def_c_index;
    protected int eye_def_smile1_index;
    protected int eye_def_def_c_index;
    protected int mth_def_smile1_index;
    protected int mth_def_sap_index;
    protected int mth_def_conf_index;
    protected int mth_def_a_index;

    protected CancellationTokenSource cts;
    protected CancellationToken destroyToken;
    protected CancellationTokenSource linkedCts;
    protected CancellationToken linkedToken;

    protected virtual void OnEnable()
    {
        blw_def_smile2_index = smr_blw_def.sharedMesh.GetBlendShapeIndex("BLW_SMILE2");
        blw_def_conf_index = smr_blw_def.sharedMesh.GetBlendShapeIndex("BLW_CONF");
        blw_def_ang1_index = smr_blw_def.sharedMesh.GetBlendShapeIndex("BLW_ANG1");
        el_def_smile1_index = smr_el_def.sharedMesh.GetBlendShapeIndex("EYE_SMILE1");
        el_def_def_c_index = smr_el_def.sharedMesh.GetBlendShapeIndex("EYE_DEF_C");
        eye_def_smile1_index = smr_eye_def.sharedMesh.GetBlendShapeIndex("EYE_SMILE1");
        eye_def_def_c_index = smr_eye_def.sharedMesh.GetBlendShapeIndex("EYE_DEF_C");
        mth_def_smile1_index = smr_mth_def.sharedMesh.GetBlendShapeIndex("MTH_SMILE1");
        mth_def_sap_index = smr_mth_def.sharedMesh.GetBlendShapeIndex("MTH_SAP");
        mth_def_conf_index = smr_mth_def.sharedMesh.GetBlendShapeIndex("MTH_CONF");
        mth_def_a_index = smr_mth_def.sharedMesh.GetBlendShapeIndex("MTH_A");

        cts = new CancellationTokenSource();
        destroyToken = this.GetCancellationTokenOnDestroy();
        linkedCts = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, cts.Token);
        linkedToken = linkedCts.Token;
    }
    protected virtual void OnDisable()
    {
        cts?.Cancel();
        linkedCts?.Dispose();
        linkedCts = null;
        cts?.Dispose();
        cts = null;
    }
    protected virtual void OnDestroy()
    {
        cts?.Dispose();
        linkedCts?.Dispose();
    }    
}
