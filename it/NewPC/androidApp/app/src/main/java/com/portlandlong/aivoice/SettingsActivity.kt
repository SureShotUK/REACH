package com.portlandlong.aivoice

import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import androidx.appcompat.app.AppCompatActivity

class SettingsActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_settings)
        supportActionBar?.apply {
            setDisplayHomeAsUpEnabled(true)
            title = "Settings"
        }

        val prefs = getSharedPreferences(MainActivity.PREFS_NAME, MODE_PRIVATE)

        val etUrl = findViewById<EditText>(R.id.etApiUrl)
        val etKey = findViewById<EditText>(R.id.etApiKey)
        val etModel = findViewById<EditText>(R.id.etModel)
        val etSearchUrl = findViewById<EditText>(R.id.etSearchUrl)
        val btnSave = findViewById<Button>(R.id.btnSave)

        etUrl.setText(prefs.getString(MainActivity.KEY_API_URL, MainActivity.DEFAULT_URL))
        etKey.setText(prefs.getString(MainActivity.KEY_API_KEY, ""))
        etModel.setText(prefs.getString(MainActivity.KEY_MODEL, MainActivity.DEFAULT_MODEL))
        etSearchUrl.setText(prefs.getString(MainActivity.KEY_SEARCH_URL, MainActivity.DEFAULT_SEARCH_URL))

        btnSave.setOnClickListener {
            prefs.edit()
                .putString(MainActivity.KEY_API_URL, etUrl.text.toString().trim())
                .putString(MainActivity.KEY_API_KEY, etKey.text.toString().trim())
                .putString(MainActivity.KEY_MODEL, etModel.text.toString().trim())
                .putString(MainActivity.KEY_SEARCH_URL, etSearchUrl.text.toString().trim())
                .apply()
            finish()
        }
    }

    override fun onSupportNavigateUp(): Boolean {
        onBackPressedDispatcher.onBackPressed()
        return true
    }
}
