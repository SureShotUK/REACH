package com.portlandlong.aivoice

import android.Manifest
import android.content.Intent
import android.content.pm.PackageManager
import android.os.Bundle
import android.speech.RecognitionListener
import android.speech.RecognizerIntent
import android.speech.SpeechRecognizer
import android.speech.tts.TextToSpeech
import android.view.Gravity
import android.view.Menu
import android.view.MenuItem
import android.widget.LinearLayout
import android.widget.ScrollView
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import androidx.lifecycle.lifecycleScope
import com.google.android.material.floatingactionbutton.FloatingActionButton
import com.google.android.material.switchmaterial.SwitchMaterial
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import org.json.JSONArray
import org.json.JSONObject
import java.io.OutputStreamWriter
import java.net.HttpURLConnection
import java.net.URL
import java.net.URLEncoder
import java.time.LocalDate
import java.util.Locale

class MainActivity : AppCompatActivity(), TextToSpeech.OnInitListener {

    private lateinit var chatContainer: LinearLayout
    private lateinit var scrollView: ScrollView
    private lateinit var micFab: FloatingActionButton
    private lateinit var ttsSwitch: SwitchMaterial
    private lateinit var statusText: TextView

    private lateinit var speechRecognizer: SpeechRecognizer
    private lateinit var tts: TextToSpeech
    private var ttsReady = false
    private var isListening = false
    private var isThinking = false

    private val conversationHistory = mutableListOf<Map<String, String>>()

    companion object {
        const val PERM_REQUEST = 1001
        const val PREFS_NAME = "AiVoicePrefs"
        const val KEY_API_URL = "api_url"
        const val KEY_API_KEY = "api_key"
        const val KEY_MODEL = "model"
        const val KEY_TTS_ENABLED = "tts_enabled"
        const val KEY_SEARCH_URL = "search_url"
        const val DEFAULT_URL = "https://amelai.tail926601.ts.net/api/chat/completions"
        const val DEFAULT_MODEL = "qwen3.6:27b"
        const val DEFAULT_SEARCH_URL = "https://amelai.tail926601.ts.net:8080"
        val SPEECH_CORRECTIONS = mapOf(
            "weatherby" to "Wetherby"
        )
        const val SYSTEM_PROMPT = "You are a concise voice assistant with web search capability. Before each response, current web search results are fetched and provided to you — use them to answer questions about recent news, current events, or live data. Give only the essential facts requested. No filler, no padding, no unnecessary context. Keep responses as short as possible while remaining accurate. Use plain language suitable for reading aloud. Do not use any special characters including asterisks, backslashes, hashtags, bullet symbols, or emojis — plain text only."
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        setSupportActionBar(findViewById<Toolbar>(R.id.toolbar))

        chatContainer = findViewById(R.id.chatContainer)
        scrollView = findViewById(R.id.scrollView)
        micFab = findViewById(R.id.micFab)
        ttsSwitch = findViewById(R.id.ttsSwitch)
        statusText = findViewById(R.id.statusText)

        val prefs = getSharedPreferences(PREFS_NAME, MODE_PRIVATE)
        ttsSwitch.isChecked = prefs.getBoolean(KEY_TTS_ENABLED, true)
        ttsSwitch.setOnCheckedChangeListener { _, isChecked ->
            prefs.edit().putBoolean(KEY_TTS_ENABLED, isChecked).apply()
            if (!isChecked && ttsReady) tts.stop()
        }

        tts = TextToSpeech(this, this)
        setupSpeechRecognizer()

        micFab.setOnClickListener {
            when {
                isThinking -> return@setOnClickListener
                isListening -> { speechRecognizer.stopListening(); setIdleState() }
                else -> startListening()
            }
        }

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.RECORD_AUDIO)
            != PackageManager.PERMISSION_GRANTED
        ) {
            ActivityCompat.requestPermissions(this, arrayOf(Manifest.permission.RECORD_AUDIO), PERM_REQUEST)
        }
    }

    override fun onInit(status: Int) {
        if (status == TextToSpeech.SUCCESS) {
            val result = tts.setLanguage(Locale.UK)
            ttsReady = result != TextToSpeech.LANG_MISSING_DATA && result != TextToSpeech.LANG_NOT_SUPPORTED
        }
    }

    private fun setupSpeechRecognizer() {
        speechRecognizer = SpeechRecognizer.createSpeechRecognizer(this)
        speechRecognizer.setRecognitionListener(object : RecognitionListener {
            override fun onReadyForSpeech(params: Bundle?) = setListeningState()
            override fun onBeginningOfSpeech() {}
            override fun onRmsChanged(rmsdB: Float) {}
            override fun onBufferReceived(buffer: ByteArray?) {}
            override fun onEndOfSpeech() {}
            override fun onPartialResults(partialResults: Bundle?) {}
            override fun onEvent(eventType: Int, params: Bundle?) {}

            override fun onResults(results: Bundle?) {
                var text = results
                    ?.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION)
                    ?.firstOrNull()
                    ?: run { setIdleState(); return }
                SPEECH_CORRECTIONS.forEach { (wrong, right) ->
                    text = text.replace(wrong, right, ignoreCase = true)
                }
                setThinkingState()
                addMessage(text, isUser = true)
                sendToAI(text)
            }

            override fun onError(error: Int) {
                setIdleState()
                if (error != SpeechRecognizer.ERROR_CLIENT) {
                    val msg = when (error) {
                        SpeechRecognizer.ERROR_NO_MATCH -> "Didn't catch that — try again"
                        SpeechRecognizer.ERROR_NETWORK -> "Network error — check Tailscale is connected"
                        SpeechRecognizer.ERROR_SPEECH_TIMEOUT -> "No speech detected"
                        else -> "Recognition error ($error)"
                    }
                    addMessage(msg, isUser = false, isError = true)
                }
            }
        })
    }

    private fun setIdleState() {
        isListening = false
        isThinking = false
        statusText.text = getString(R.string.tap_mic)
        micFab.backgroundTintList = getColorStateList(R.color.fab_idle)
        micFab.isEnabled = true
    }

    private fun setListeningState() {
        isListening = true
        statusText.text = getString(R.string.listening)
        micFab.backgroundTintList = getColorStateList(R.color.fab_listening)
    }

    private fun setThinkingState() {
        isListening = false
        isThinking = true
        statusText.text = getString(R.string.thinking)
        micFab.backgroundTintList = getColorStateList(R.color.fab_thinking)
        micFab.isEnabled = false
    }

    private fun startListening() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.RECORD_AUDIO)
            != PackageManager.PERMISSION_GRANTED
        ) {
            ActivityCompat.requestPermissions(this, arrayOf(Manifest.permission.RECORD_AUDIO), PERM_REQUEST)
            return
        }
        val intent = Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH).apply {
            putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM)
            putExtra(RecognizerIntent.EXTRA_LANGUAGE, "en-GB")
            putExtra(RecognizerIntent.EXTRA_MAX_RESULTS, 1)
        }
        speechRecognizer.startListening(intent)
    }

    private fun sendToAI(userText: String) {
        val prefs = getSharedPreferences(PREFS_NAME, MODE_PRIVATE)
        val apiUrl = prefs.getString(KEY_API_URL, DEFAULT_URL) ?: DEFAULT_URL
        val apiKey = prefs.getString(KEY_API_KEY, "") ?: ""
        val model = prefs.getString(KEY_MODEL, DEFAULT_MODEL) ?: DEFAULT_MODEL
        val searchUrl = prefs.getString(KEY_SEARCH_URL, DEFAULT_SEARCH_URL) ?: DEFAULT_SEARCH_URL

        conversationHistory.add(mapOf("role" to "user", "content" to userText))

        lifecycleScope.launch {
            val result = withContext(Dispatchers.IO) {
                val searchContext = try { searchWeb(searchUrl, userText) } catch (e: Exception) { null }
                callApi(apiUrl, apiKey, model, conversationHistory, searchContext)
            }
            setIdleState()
            result.fold(
                onSuccess = { response ->
                    conversationHistory.add(mapOf("role" to "assistant", "content" to response))
                    addMessage(response, isUser = false)
                    if (ttsSwitch.isChecked && ttsReady) {
                        tts.speak(response, TextToSpeech.QUEUE_FLUSH, null, "msg_${System.currentTimeMillis()}")
                    }
                },
                onFailure = { error ->
                    conversationHistory.removeLastOrNull()
                    addMessage("Error: ${error.message}", isUser = false, isError = true)
                }
            )
        }
    }

    private fun searchWeb(searchUrl: String, query: String): String? {
        val encoded = URLEncoder.encode(query, "UTF-8")
        val conn = (URL("$searchUrl/search?q=$encoded&format=json&categories=general").openConnection() as HttpURLConnection).apply {
            connectTimeout = 5_000
            readTimeout = 5_000
        }
        if (conn.responseCode != 200) return null
        val results = JSONObject(conn.inputStream.bufferedReader().readText()).getJSONArray("results")
        val count = minOf(3, results.length())
        if (count == 0) return null
        val sb = StringBuilder("Current web search results:\n")
        for (i in 0 until count) {
            val r = results.getJSONObject(i)
            val title = r.optString("title", "")
            val content = r.optString("content", "")
            if (title.isNotBlank() || content.isNotBlank()) sb.append("- $title: $content\n")
        }
        return sb.toString()
    }

    private fun callApi(
        apiUrl: String,
        apiKey: String,
        model: String,
        history: List<Map<String, String>>,
        searchContext: String? = null
    ): Result<String> = runCatching {
        val systemContent = buildString {
            append(SYSTEM_PROMPT)
            append(" Today's date: ${LocalDate.now()}.")
            if (searchContext != null) append("\n\n$searchContext")
        }

        val conn = (URL(apiUrl).openConnection() as HttpURLConnection).apply {
            requestMethod = "POST"
            setRequestProperty("Content-Type", "application/json")
            if (apiKey.isNotBlank()) setRequestProperty("Authorization", "Bearer $apiKey")
            doOutput = true
            connectTimeout = 15_000
            readTimeout = 120_000
        }

        val messages = JSONArray().apply {
            put(JSONObject().apply {
                put("role", "system")
                put("content", systemContent)
            })
            for (msg in history) put(JSONObject().apply {
                put("role", msg["role"])
                put("content", msg["content"])
            })
        }
        val body = JSONObject().apply {
            put("model", model)
            put("messages", messages)
            put("stream", false)
        }.toString()

        OutputStreamWriter(conn.outputStream, "UTF-8").use { it.write(body) }

        if (conn.responseCode != 200) {
            val err = conn.errorStream?.bufferedReader()?.readText() ?: "no error body"
            throw Exception("HTTP ${conn.responseCode}: $err")
        }

        JSONObject(conn.inputStream.bufferedReader().readText())
            .getJSONArray("choices")
            .getJSONObject(0)
            .getJSONObject("message")
            .getString("content")
    }

    private fun addMessage(text: String, isUser: Boolean, isError: Boolean = false) {
        val dp = resources.displayMetrics.density
        val pad = (12 * dp).toInt()
        val margin = (8 * dp).toInt()
        val wideMargin = (48 * dp).toInt()

        val tv = TextView(this).apply {
            this.text = text
            textSize = 15f
            setPadding(pad, (pad * 0.6f).toInt(), pad, (pad * 0.6f).toInt())
            setBackgroundResource(if (isUser) R.drawable.bg_user_message else R.drawable.bg_ai_message)
            setTextColor(when {
                isError -> 0xFFB71C1C.toInt()
                isUser -> 0xFF1A237E.toInt()
                else -> 0xFF212121.toInt()
            })
            layoutParams = LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WRAP_CONTENT,
                LinearLayout.LayoutParams.WRAP_CONTENT
            ).apply {
                gravity = if (isUser) Gravity.END else Gravity.START
                setMargins(
                    if (isUser) wideMargin else margin,
                    margin / 2,
                    if (isUser) margin else wideMargin,
                    margin / 2
                )
            }
        }

        chatContainer.addView(tv)
        scrollView.post { scrollView.fullScroll(ScrollView.FOCUS_DOWN) }
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        menuInflater.inflate(R.menu.main_menu, menu)
        return true
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean = when (item.itemId) {
        R.id.action_settings -> {
            startActivity(Intent(this, SettingsActivity::class.java))
            true
        }
        R.id.action_clear -> {
            conversationHistory.clear()
            chatContainer.removeAllViews()
            true
        }
        else -> super.onOptionsItemSelected(item)
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == PERM_REQUEST && grantResults.firstOrNull() == PackageManager.PERMISSION_GRANTED) {
            addMessage("Microphone permission granted — tap the mic button to start", isUser = false)
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        speechRecognizer.destroy()
        if (ttsReady) { tts.stop(); tts.shutdown() }
    }
}
