# antiCrackC#

An anti-crack system in C# that detects forbidden/open programs running on the user’s device and sends logs to a Discord webhook when a prohibited program is detected.

## Overview

This project monitors running processes to identify any forbidden software that might be used for cracking or unauthorized access. When such software is detected, a detailed log is sent to a configured Discord webhook for immediate notification.

## Features

- Continuously scans running processes for forbidden program names.
- Sends detailed logs with detection time and process info to a Discord webhook.
- Helps prevent unauthorized program usage or cracking attempts.
- Easy to configure forbidden programs list and webhook URL.

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/NoMoreLife-1/antiCrackCSharp.git
