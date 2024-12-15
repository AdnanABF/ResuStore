function UserDataViewModel(usersData) {
    var self = this;
    self.users = ko.observableArray(usersData);
    self.selectedUser = ko.observable({
        Id: '',
        FirstName: '',
        LastName: '',
        Email: '',
        DateOfBirth: '',
        Gender: '',
        ImageUrl: ''
    });

    self.firstName = ko.observable('');
    self.lastName = ko.observable('');
    self.gender = ko.observable('');
    self.email = ko.observable('');
    self.dateOfBirth = ko.observable('');

    self.getAllUsers = function (callBack) {
        $.ajax({
            url: "/Users/GetUsersJsonData",
            type: 'GET',
            dataType: 'json',
            success: function (result) {
                self.users(result);
                if (callBack) {
                    callBack();
                }
            },
            error: function (xhr, status, error) {
                console.error("Failed to fetch users : ", error);
            }
        });
    };

    self.showUserDetails = function (data) {
        $.ajax({
            url: "/Users/GetUserDetails?id=" + data.Id,
            type: 'GET',
            dataType: 'json',
            success: function (result) {
                self.selectedUser(result);
                $('#userDetailModal').modal('show');
            },
            error: function (xhr, status, error) {
                console.error("Failed to fetch user details : ", error);
            }
        });
    };

    self.createUser = function () {
        var form = document.getElementById('createUser')
        var formData = new FormData(form);

        $.ajax({
            url: "/Users/Create",
            type: 'POST',
            processData: false,
            contentType: false,
            data: formData,
            success: function (result) {
                if (result.success) {
                    self.updateUserTiles();
                    self.resetForm();
                    $('#createUserModal').modal('hide');
                } else {
                    $('.text-danger').text('');

                    for (var key in result.errors) {
                        if (result.errors.hasOwnProperty(key)) {
                            var errorMessages = result.errors[key];
                            $('#' + key + 'Error').text(errorMessages.join(', '));
                        }
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error("Error occured : ", error);
            }
        });
    };

    self.deleteUserPopUp = function () {
        $('.userModalFooter').hide();
        $('.deleteConfirmation').show();
    }

    self.deleteUser = function () {
        var form = document.getElementById('deleteUser')
        var formData = new FormData(form);

        $.ajax({
            url: "/Users/Delete",
            type: 'POST',
            processData: false,
            contentType: false,
            data: formData,
            success: function (result) {
                if (result.success) {
                    self.updateUserTiles();
                    $('#userDetailModal').modal('hide');
                }
                else {
                    console.error("Failed to delete user : ", result.errors);
                }
            },
            error: function (xhr, status, error) {
                console.error("Failed to delete user : ", error);
            }
        });
    };

    self.updateUserTiles = function () {
        $('.addUserTile').remove();
        self.getAllUsers(function () {
            self.addCreateUserTile();
        });
    };

    $('#createUserModal').on('hidden.bs.modal', function () { self.resetForm(); });
    $('#userDetailModal').on('hidden.bs.modal', function () {
        self.selectedUser({
            Id: '',
            FirstName: '',
            LastName: '',
            Email: '',
            DateOfBirth: '',
            Gender: '',
            ImageUrl: ''
        });
        $('.userModalFooter').show();
        $('.deleteConfirmation').hide();
    });

    self.resetForm = function () {
        self.firstName('');
        self.lastName('');
        self.gender('');
        self.email('');
        self.dateOfBirth('');
        $('#imageFile').val('');
        $('span[id$="Error"]').text('');
    };

    self.addCreateUserTile = function () {
        var tileHtml = $("<div class=\"userContainer p-2 addUserTile\"><div class=\"userTile emptyTile\"><button id=\"btnAddTile\" class=\"btn btn-primary tileEmptyBtn\" data-bs-toggle=\"modal\" data-bs-target=\"#createUserModal\" aria-label=\"Add Tile\" >Add</button></div></div>");
        $('#userContainerWrap').prepend(tileHtml);
    };
}

$(document).ready(function () {
    var viewModel = new UserDataViewModel(initialUsersData);
    ko.applyBindings(viewModel);
    viewModel.addCreateUserTile();
});