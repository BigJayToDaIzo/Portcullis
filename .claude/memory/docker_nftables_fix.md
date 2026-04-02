---
name: Docker broken — kernel version mismatch after update
description: Docker fails to start because running kernel (6.19.9) doesn't match installed kernel (6.19.10). Modules like nf_nat not found. Reboot required.
type: project
---

Docker is in `failed (start-limit-hit)` state as of 2026-03-28.

**Root cause**: Kernel version mismatch. Running `6.19.9-arch1-1` but `linux 6.19.10.arch1-1` is installed. Module files on disk are for 6.19.10, so `modprobe nf_nat` fails with "not found in directory." Docker's bridge driver needs `nf_nat` for NAT/PREROUTING chains.

The dockerd error was:
- `iptables v1.8.11 (nf_tables): CHAIN_ADD failed (No such file or directory): chain PREROUTING`
- `Warning: Extension addrtype revision 0 not supported, missing kernel module?`

**Why:** topgrade updated the kernel package from 6.19.9 to 6.19.10 but the system hasn't been rebooted. Wireguard VPN config changes may have also been a factor but the kernel mismatch is the primary blocker.

**How to apply — fix steps:**
1. **Reboot** — gets onto 6.19.10 where all modules match
2. Docker should start on its own (if enabled) or: `sudo systemctl start docker`
3. If Docker still fails post-reboot, *then* investigate nftables config (Wireguard changes)

**Blocking**: Phase 2 Testcontainers tests (2 tests fail without Docker). Phase 1 entity tests (21) unaffected.
