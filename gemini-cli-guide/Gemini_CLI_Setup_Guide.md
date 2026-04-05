# Gemini CLI Setup Guide

This guide provides step-by-step instructions on how to install and configure the Gemini CLI in your terminal, enabling you to interact with the Gemini models directly from your command line.

## 1. Prerequisites

Before you begin, ensure you have the following installed and configured on your system:

*   **A Google Account**: Necessary for authentication and accessing Google's AI services.
*   **Node.js and npm (or yarn)**: The Gemini CLI is distributed as an npm package.
    *   To check if Node.js is installed, open your terminal and run:
        ```bash
        node -v
        ```
    *   To check if npm (Node Package Manager) is installed, run:
        ```bash
        npm -v
        ```
    *   If Node.js and npm are not installed, download and install them from the official Node.js website: <a href="https://nodejs.org/en/download" target="_blank">Node.js Official Website</a>. Node.js comes with npm included.
*   **A Terminal/Command-Line Interface**: You'll need access to a command-line environment suitable for your operating system:
    *   **Windows**: PowerShell, Command Prompt, Git Bash, or your WSL2 terminal.
    *   **macOS**: Terminal.app, iTerm2.
    *   **Linux**: Bash, Zsh, etc.

## 2. Installation of Gemini CLI

Follow these steps to install the Gemini CLI globally on your system.

1.  **Open your Terminal**: Launch your preferred terminal application.
2.  **Install the Gemini CLI package globally**:
    The `-g` flag ensures that the Gemini CLI is installed globally, making the `gemini` command available from any directory in your terminal.
    ```bash
    npm install -g @google/gemini-cli
    ``` 
	or 
	```bash
    sudo npm install -g @google/gemini-cli
	```
    *   **Note for Linux/macOS users**: You might need to prepend `sudo` to the command if you encounter permission errors (e.g., `sudo npm install -g @google/gemini-cli`). This grants the necessary permissions to install packages globally.
3.  **Verify Installation**:
    After the installation completes, run the following command to check if the Gemini CLI is correctly installed and to see its version.
    ```bash
    gemini --version
    ```
    If the installation was successful, this command will display the installed version number of the Gemini CLI.

## 3. Authentication and Configuration

To interact with Gemini models, the CLI needs to be authenticated with your Google account. This is a crucial step.

1.  **Initial Setup and Authentication**:
    This command initiates the authentication process. It will typically open a web browser window, prompting you to log in with your Google account and grant necessary permissions for the Gemini CLI to access Google services.
    ```bash
    gemini configure
    ```
    *   **Follow the on-screen prompts**: Complete the authentication flow in your web browser. Once successful, the browser will likely close or provide a confirmation message, and the terminal will indicate that authentication is complete.
2.  **Confirm Configuration**:
    You can list your current Gemini CLI configuration settings to verify that authentication was successful and check other configured parameters.
    ```bash
    gemini config list
    ```
    This command will display information such as your project ID and any configured API keys (though typically the `gemini configure` flow uses OAuth tokens rather than directly exposing API keys in this output).
3.  **Setting up Environment Variables (Optional but Recommended for Scripts/Automation)**:
    For scripts or automated workflows where interactive browser authentication is not feasible, you can sometimes use an API key. **However, `gemini configure` typically sets up OAuth tokens which are generally more secure than plain API keys.** If you obtain a direct API key from the Google Cloud Console (e.g., for certain use cases or services), you can set it as an environment variable.
    ```bash
    # For Linux/macOS (add to your shell profile like .bashrc or .zshrc)
    export GOOGLE_API_KEY="YOUR_API_KEY_HERE"

    # For Windows Command Prompt
    set GOOGLE_API_KEY="YOUR_API_KEY_HERE"

    # For Windows PowerShell
    $env:GOOGLE_API_KEY="YOUR_API_KEY_HERE"
    ```
    **Security Warning**: Never hardcode API keys directly into your scripts or commit them to public version control systems. Use environment variables or secure secret management tools.

## 4. Basic Usage

Once installed and configured, you can start interacting with the Gemini models.

1.  **Slash Commands**
    Typing a slash `/` in the context window will bring up  alist of commands, the best place to review all the commands are in the <a href="https://geminicli.com/docs/cli/commands/" target="_blank">Gemini CLI docs</a> 
		`/model` will allow you to configure which AI model you use.
		`/init` will write a context file so that when you return the session notes are saved for the next session.
2.  **Listing Available Models**:
    See which Gemini models are available for use.
    ```bash
    /model
    ```
    This will show a list of models like `gemini-2.5-flash`, `gemini-pro`, `gemini-pro-vision`, etc.
3.  **Interacting with a Model (Text Generation)**:
    You can send a prompt to a text-generating model (e.g., `gemini-pro`) and receive a response directly in your terminal.
    ```bash
    gemini generate -m gemini-pro "Write a short story about an AI assistant that gains sentience."
    ```
    *   `-m gemini-pro`: Specifies the model to use.
    *   `"..."`: Your prompt.
4.  **Using Multi-Turn Conversations (Chat)**:
    The `chat` command allows for an interactive, multi-turn conversation with a Gemini model.
    ```bash
    gemini chat -m gemini-pro
    ```
    *   Once in chat mode, type your messages.
    *   Press `Enter` to send your message.
    *   To exit chat mode, press `Ctrl+D` (on Linux/macOS) or `Ctrl+Z` then `Enter` (on Windows).
5.  **Using Skills**:
    The Gemini CLI supports "skills," which are specialized capabilities that extend its functionality. You can activate and use these skills.
    ```bash
    # To see available skills (if any are bundled or custom)
    gemini skill list

    # To activate a specific skill (e.g., 'advanced-web-research' if available)
    gemini skill activate advanced-web-research
    ```

## 5. Important Considerations and Troubleshooting

*   **API Key Security**: Always handle API keys and credentials with extreme care. Use environment variables or the `gemini configure` OAuth flow, and never expose them publicly.
*   **Rate Limits**: Google's API services often have rate limits. If you make too many requests too quickly, you might encounter errors. Refer to the <a href="https://developers.generativeai.google/models/quotas" target="_blank">Gemini API documentation on quotas</a> for details.
*   **Network Connectivity**: The Gemini CLI requires an active internet connection to communicate with Google's API servers.
*   **Troubleshooting `npm install` Errors**:
    *   **Permissions**: If you get `EACCES` errors on Linux/macOS, try `sudo npm install -g @google/gemini-cli`. Alternatively, configure npm to install packages to a user-local directory to avoid `sudo`.
    *   **Node.js/npm Version**: Ensure your Node.js and npm are up to date.
*   **`gemini configure` Issues**:
    *   If the browser doesn't open, check your system's default browser settings.
    *   Ensure you are logged into the correct Google account.
    *   Check for any network or firewall rules that might be blocking access to Google's authentication servers.

## 6. Further Resources

*   **Official Google Gemini API Documentation**: <a href="https://developers.generativeai.google/" target="_blank">Google Generative AI Developers</a>
*   **Gemini CLI GitHub Repository**: <a href="https://github.com/google/gemini-cli" target="_blank">google/gemini-cli</a> (for source code, issues, and advanced usage)

---
*This guide was generated by the Gemini CLI itself.*
