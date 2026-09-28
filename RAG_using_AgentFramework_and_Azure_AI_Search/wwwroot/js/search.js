

window.document.addEventListener("DOMContentLoaded", function () {
    const searchForm = document.getElementById("search-form");
    const agenticSearch = document.getElementById("agenticSearch");
    const spinner = document.getElementById("spinner");
    const includeCitations = document.getElementById("IncludeCitations");
    searchForm.addEventListener("submit", function (event) {

        var url = '/ai-search';
        event.preventDefault();
        let citations = false;

        if (agenticSearch.checked)
        {
            url = '/agentic-ai-search';
        };

        if (includeCitations && includeCitations.checked) {
            citations = true;
        }

        const input = document.getElementById("Input");
        const topK = document.getElementById("TopK");
        spinner.classList.remove("visually-hidden");
        fetch(url, {
            method: "POST",
            body: JSON.stringify({
                input : input.value,
                topK: topK.value,
                includeCitations: citations
            }),
            headers: {
                "Content-Type": "application/json"
            }
        }).then(response => response.json())
            .then(data => {
                const searchResult = document.getElementById("searchResult");
                searchResult.innerHTML = data.response;
                spinner.classList.add("visually-hidden");
            })
    });

}); 




