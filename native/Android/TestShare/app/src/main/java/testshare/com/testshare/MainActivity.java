package testshare.com.testshare;

import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.net.Uri;
import android.os.Environment;
import android.support.v4.content.FileProvider;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.widget.Button;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;


public class MainActivity extends AppCompatActivity implements View.OnClickListener {

    private Button buttonShareText;
    private Button buttonShareUrl;
    private Button buttonShareBitmapOld;
    private Button buttonShareBitmapNew;
    private Button buttonShareAllNew;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        buttonShareText = (Button) findViewById(R.id.buttonShareText);
        buttonShareUrl = (Button) findViewById(R.id.buttonShareUrl);
        buttonShareBitmapOld = (Button) findViewById(R.id.buttonShareBitmapOld);
        buttonShareBitmapNew = (Button) findViewById(R.id.buttonShareBitmapNew);
        buttonShareAllNew = (Button) findViewById(R.id.buttonShareAllNew);

        buttonShareText.setOnClickListener(this);
        buttonShareUrl.setOnClickListener(this);
        buttonShareBitmapOld.setOnClickListener(this);
        buttonShareBitmapNew.setOnClickListener(this);
        buttonShareAllNew.setOnClickListener(this);

    }

    @Override
    public void onClick(View v) {

        if (v == buttonShareText) {

            share("SHARE TEXT", null);

        } else if (v == buttonShareUrl) {

            share("SHARE URL", "https://www.google.com/");

        } else if (v == buttonShareBitmapOld) {

            share("SHARE BITMAP (old)", null, newTestBitmap(), false);

        } else if (v == buttonShareBitmapNew) {

            share("SHARE BITMAP (new)", null, newTestBitmap(), true);

        } else if (v == buttonShareAllNew) {

            share("SHARE ALL (new)", "https://www.google.com/", newTestBitmap(), true);
        }

    }

    private void share(String text, String url) {
        share(text, url, null, false);
    }

    private void share(String message, String url, Bitmap image, boolean isShareMethodNew) {

        Intent sendIntent = new Intent();

        sendIntent.setAction(Intent.ACTION_SEND);

        if (image == null) {
            sendIntent.setType("text/plain");
        } else {
            sendIntent.setType("image/png");
        }

        sendIntent.putExtra(Intent.EXTRA_SUBJECT, "My app name");

        if (!TextUtils.isEmpty(message) || !TextUtils.isEmpty(url)) {

            String text;
            if (TextUtils.isEmpty(message)) {
                text = url;
            } else if (TextUtils.isEmpty(url)) {
                text = message;
            } else {
                text = message + "\n" + url;
            }

            sendIntent.putExtra(Intent.EXTRA_TEXT, text);
        }


        if (image != null) {

            File imageFileToShare = null;

            try {
                imageFileToShare = File.createTempFile(
                        "share-image",
                        ".png",
                        getExternalCacheDir());

            } catch (IOException e) {
                e.printStackTrace();
            }

            if (writeImageToFile(image, imageFileToShare)) {

                Uri uri;

                if (!isShareMethodNew) {
                    //old : write to file then add file URI to share intent
                    uri = Uri.fromFile(imageFileToShare);

                } else {
                    //new
                    uri = FileProvider.getUriForFile(MainActivity.this,
                            "com.testshare.fileprovider",
                            imageFileToShare);
                }

                sendIntent.putExtra(Intent.EXTRA_STREAM, uri);

            }
        }

        startActivity(sendIntent);

    }

    private static boolean writeImageToFile(Bitmap image, File file) {

        FileOutputStream fos = null;
        try {
            fos = new FileOutputStream(file);
            image.compress(Bitmap.CompressFormat.PNG, 100, fos);

        } catch (Exception e) {

            e.printStackTrace();
            return false;

        } finally {

            if (fos != null) {
                try {
                    fos.close();
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        }

        return true;
    }

    private static Bitmap newTestBitmap() {

        final int width = 15;
        final int height = 10;

        Bitmap res = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);

        for (int y = 0 ; y < height ; y++) {

            for (int x = 0 ; x < width ; x++) {

                int color;

                if (x < width * 0.33f) {
                    color = Color.RED;
                } else if (x < width * 0.67f) {
                    color = Color.WHITE;
                } else {
                    color = Color.BLUE;
                }

                res.setPixel(x, y, color);
            }
        }

        return res;
    }

}
