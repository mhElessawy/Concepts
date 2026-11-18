const accountTree = document.getElementById('category-account-tree');
const accountOptions = document.getElementById('category-account-options');
const accountId = document.getElementById('AccountId');
const accountLabel = document.getElementById('category-account-label');
let treeLoaded = false;

document.getElementById('category-cancel').addEventListener('click', event => {
    if (!window.confirm('Are you sure you want to CANCEL this feature?')) {
        event.preventDefault();
    }
});

document.getElementById('category-account-clear').addEventListener('click', () => {
    accountId.value = '';
    accountLabel.value = '';
});

document.getElementById('category-account-close').addEventListener('click', () => accountTree.close());

document.getElementById('category-account-open').addEventListener('click', async () => {
    accountTree.showModal();
    if (treeLoaded) return;

    accountOptions.textContent = 'Loading accounts…';
    try {
        const response = await fetch(accountTree.dataset.url);
        if (!response.ok) throw new Error('Account tree request failed');
        const { branches, accounts } = await response.json();

        const branchesByParent = groupByParent(branches);
        const accountsByParent = groupByParent(accounts);
        accountOptions.replaceChildren();
        if (!accounts.length) {
            accountOptions.textContent = 'No active accounts available.';
        } else {
            renderAccounts(accountOptions, null, branchesByParent, accountsByParent);
        }
        treeLoaded = true;
    } catch {
        accountOptions.textContent = 'Could not load accounts. Close and reopen the account tree to try again.';
    }
});

function groupByParent(nodes) {
    const groups = new Map();
    for (const node of nodes) {
        if (!groups.has(node.parentAccountId)) groups.set(node.parentAccountId, []);
        groups.get(node.parentAccountId).push(node);
    }
    return groups;
}

function renderAccounts(container, parentId, branches, accounts) {
    for (const branch of branches.get(parentId) ?? []) {
        const details = document.createElement('details');
        details.className = 'ms-2 mb-1';
        const summary = document.createElement('summary');
        summary.textContent = labelFor(branch);
        details.append(summary);
        renderAccounts(details, branch.id, branches, accounts);
        container.append(details);
    }

    for (const account of accounts.get(parentId) ?? []) {
        const button = document.createElement('button');
        button.type = 'button';
        button.className = 'btn btn-link d-block text-start ms-2 py-1';
        button.textContent = labelFor(account);
        button.addEventListener('click', () => {
            accountId.value = account.id;
            accountLabel.value = button.textContent;
            accountTree.close();
        });
        container.append(button);
    }
}

function labelFor(account) {
    return account.accountNo + ' — ' + account.accountName;
}
