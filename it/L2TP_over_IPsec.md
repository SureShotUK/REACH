Setting Up an L2TP over IPsec VPN

This document provides a comprehensive, step-by-step guide for configuring a Vigor router to function as a VPN server, enabling remote users to connect to the private network using the L2TP (Layer 2 Tunneling Protocol) over IPsec protocol. The instructions are derived from the Vigor2865 Series User's Guide.

1. Core Concepts

* Virtual Private Network (VPN): A VPN extends a private network across a public network, like the Internet. It allows users to send and receive data as if their devices were directly connected to the private network.
* L2TP VPN Service: This is a tunneling protocol used in VPNs. It does not provide encryption itself and is typically used in conjunction with IPsec to secure the data tunnel.
* IPsec (Internet Protocol Security): A network protocol suite that authenticates and encrypts data packets. When combined with L2TP, it creates a secure VPN connection. IPsec configuration involves two phases:
  * Phase 1: Negotiation of IKE (Internet Key Exchange) parameters, including encryption, hash, and Diffie-Hellman values, to protect the connection. Authentication is handled via a Pre-Shared Key or a Digital Signature.
  * Phase 2: Negotiation of IPsec security methods, including Authentication Header (AH) for data integrity or Encapsulating Security Payload (ESP) for confidentiality and integrity.
* Remote Access (Dial-in): This VPN configuration allows individual remote hosts (teleworkers) to connect to the main office's LAN. The remote host is assigned an IP address from the local subnet.

2. Prerequisite: Enable L2TP VPN Service

Before configuring specific user profiles, the L2TP VPN service must be enabled on the router.

1. Navigate to VPN and Remote Access >> Remote Access Control.
2. In the Remote Access Control Setup table, check the box for Enable L2TP VPN Service.
3. Navigate to the Bind to WAN section (VPN and Remote Access >> Remote Access Control >> Bind to WAN tab).
4. Ensure the L2TP VPN Service is checked for the WAN interfaces that should accept incoming VPN connections.
5. Click OK to save the changes.

3. Configuration Method 1: Using the VPN Server Wizard

The VPN Server Wizard provides a guided, step-by-step process for creating a remote dial-in user profile for L2TP over IPsec.

Step 1: Launch the Wizard Navigate to Wizards >> VPN Server Wizard from the main menu bar.

Step 2: Choose VPN Establishment Environment

1. On the first screen, for VPN Server Mode Selection, select Remote Dial-in User (Teleworker). This configures the router to accept inbound connections from remote users.
2. Click Next.

Step 3: Select Allowed Dial-in Type

1. From the list of Allowed Dial-in Type options, check the box for L2TP with IPsec Policy.
2. A dropdown menu will appear. Select the desired IPsec enforcement level:
  * Nice to Have: IPsec is preferred but not mandatory for the connection.
  * Must: IPsec is required to establish the L2TP connection.
3. Click Next. A dialog box may appear reminding you to configure a common preshared key if using IPsec Main mode and the remote gateway has a dynamic IP; click OK to proceed.

Step 4: Configure VPN Authentication Settings The VPN Authentication Setting screen for L2TP with IPsec Authentication (Nice to Have/Must) will appear. Fill in the following fields:

Field	Description
Username	Enter the username for the remote user. The maximum length is 11 characters.
Password	Enter the password for the remote user. The maximum length is 11 characters.
Pre-Shared Key	Enter the Pre-Shared Key (PSK) that the remote user's client must use for IPsec authentication. The PSK can be up to 64 characters long.
Confirm Pre-Shared Key	Re-enter the Pre-Shared Key to confirm.

Click Next to proceed.

Step 5: Finalize and Confirm

1. A confirmation page will display a summary of your configured settings. Review the information to ensure it is correct.
2. If any changes are needed, click Back.
3. To save the profile and complete the setup, click Finish. You will be presented with the following options:
  * Go to the VPN Connection Management: Proceeds to the VPN and Remote Access >> Connection Management page to manage active VPN sessions.
  * Do another VPN Server Wizard Setup: Reruns the wizard to configure another profile.
  * View more detailed configurations: Opens the full profile in VPN and Remote Access >> LAN to LAN for advanced changes.

4. Configuration Method 2: Manual Profile Setup

For more granular control, L2TP over IPsec can be configured manually by creating or editing a Remote Dial-in User profile.

Step 1: Navigate to Remote Dial-in User Profiles Go to VPN and Remote Access >> Remote Dial-in User.

Step 2: Select a Profile Click on an unused index number to create a new profile, or click an existing profile index to edit it.

Step 3: Basic Profile Settings

1. Check the box for Enable this account.
2. Optionally, check Multiple Concurrent Connections Allowed if the same user credentials need to connect from multiple devices simultaneously.

Step 4: Configure Allowed Dial-in Type

1. In the Allowed Dial-in Type section, find the L2TP with IPsec Policy option.
2. Select the desired policy level from the dropdown menu:
  * None: Does not apply the IPsec policy. The VPN connection will be a pure L2TP connection without IPsec encryption.
  * Nice to Have: The router will attempt to apply the IPsec policy during negotiation, but if it fails, the connection will fall back to a pure L2TP connection.
  * Must: The IPsec policy is definitively applied to the L2TP connection. The connection will fail if IPsec cannot be established.

Step 5: Set User Authentication Credentials In the User Account and Authentication section, enter the following:

* Username: A unique username for this profile.
* Password: A password for authentication. The length is limited to 19 characters for L2TP.

Step 6: Configure IKE Authentication Method

1. In the IKE Authentication Method section, check the box for Pre-Shared Key.
2. Enter an IKE Pre-Shared Key. This is the secret key used for the IPsec IKE phase. The key can be 1-63 characters long.

Step 7: Configure Subnet and Save

1. Under the Subnet section, specify how the VPN client will receive an IP address (e.g., from the DHCP pool of a specific LAN).
2. Click OK at the bottom of the page to save the profile.

5. Global IPsec Configuration for Dial-in Users

For remote dial-in users who may have dynamic IP addresses, a global pre-shared key can be configured, which simplifies client setup.

1. Navigate to VPN and Remote Access >> IPsec General Setup.
2. This page provides dial-in settings for remote users. In the IKE Authentication Method section, enter values for:
  * General Pre-Shared Key: Define the PSK for general authentication.
  * Confirm General Pre-Shared Key: Re-enter the PSK to confirm.
3. Click OK to save the settings. If a user profile does not have a specific Peer ID defined, the settings on this page will be applied.
