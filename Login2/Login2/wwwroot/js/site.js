$.ajax({
    url: '/Jsondb/duyurular.json',
    type: 'GET',
    dataType: 'json',
    cache: false,
    success: function (data) {
        console.log('Received data:', data);

        var announcementsContainer = $('#announcements');
        announcementsContainer.empty();

        data.duyurular.forEach(function (duyuru) {
            if (duyuru.isChecked) {
                announcementsContainer.append(
                    '<li><textarea class="readonly-textbox" readonly>' + duyuru.text + '</textarea></li>'
                );
            }
        });
    },
    error: function (err) {
        console.log('Error fetching JSON data', err);
    }
});
