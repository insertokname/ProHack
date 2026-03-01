#!/usr/bin/env python3
"""
run_frida.py - Run a Frida script against a process and auto-exit when done.

Usage:  python run_frida.py <pid> <script.js>

The JS script should call  send({ type: 'exit' })  when it's finished.
console.log() output prints to the terminal normally.
"""
import sys
import time

import frida


def main():
    if len(sys.argv) < 3:
        print(f"Usage: python {sys.argv[0]} <pid> <script.js>")
        sys.exit(1)

    pid = int(sys.argv[1])
    script_path = sys.argv[2]

    with open(script_path, "r", encoding="utf-8") as f:
        source = f.read()

    done = False

    def on_message(message, data):
        nonlocal done
        if message["type"] == "send":
            payload = message.get("payload", "")
            if isinstance(payload, dict) and payload.get("type") == "exit":
                done = True
                return
            # Regular send() payloads — print them
            print(str(payload), flush=True)
        elif message["type"] == "error":
            desc = message.get("description", "")
            stack = message.get("stack", "")
            print(f"[Script Error] {desc}", file=sys.stderr, flush=True)
            if stack:
                print(stack, file=sys.stderr, flush=True)

    print(f"[*] Attaching to PID {pid}...")
    session = frida.attach(pid)
    script = session.create_script(source)
    script.on("message", on_message)
    script.load()

    try:
        while not done:
            time.sleep(0.2)
    except KeyboardInterrupt:
        print("\n[*] Interrupted by user.")
    finally:
        try:
            script.unload()
        except Exception:
            pass
        session.detach()

    print("[*] Frida session ended.")

if __name__ == "__main__":
    main()
