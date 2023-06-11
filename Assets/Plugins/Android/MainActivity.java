package com.zandl.androidchat;
 

import android.Manifest;
import android.content.Intent;
import android.database.Cursor;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.provider.MediaStore;
import android.util.Log;
import android.widget.Toast;
 
import android.app.Activity;
 
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;
 
 
public class MainActivity extends UnityPlayerActivity {
 
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // 获取存储权限,不然的话无法获取图片
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            requestPermissions(new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE, Manifest.permission.READ_EXTERNAL_STORAGE}, 100);
        }
    }
    // unity点击按钮触发这个方法
    public void startPhoto() {
        Log.d("unity","打开相册");
        Intent intent = new Intent(Intent.ACTION_PICK);
        intent.setType("image/*");
        startActivityForResult(intent, 123456);  // 第二个参数是请求码
    }
 
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (resultCode == RESULT_OK) {
            switch (requestCode) {
                case 123456:  // 请求码
                    Log.d("Unity", "相册返回");
 
                    UnityPlayer.UnitySendMessage("Main Camera", "CallUnity", GetPath(data));
                    break;
            }
        }
    }
 
 
    public String GetPath(Intent data) {
        Uri uri = data.getData();
        String imagePath;
        // 第二个参数是想要获取的数据
        Cursor cursor = getContentResolver()
                .query(uri, new String[]{MediaStore.Images.ImageColumns.DATA},
                        null, null, null);
        if (cursor == null) {
            imagePath = uri.getPath();
        } else {
            cursor.moveToFirst();
            // 获取数据所在的列下标
            int index = cursor.getColumnIndex(MediaStore.Images.ImageColumns.DATA);
            imagePath = cursor.getString(index);  // 获取指定列的数据
            cursor.close();
        }
        return imagePath;  // 返回图片地址
    }
 
 
    //让Unity调用的方法
    public void CallAndroid(String Msg)
    {
 
        Toast.makeText(MainActivity.this, Msg, Toast.LENGTH_SHORT).show();
 
    }
 
 
}
 
 