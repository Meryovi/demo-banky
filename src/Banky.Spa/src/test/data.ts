import { components } from "../schema";

export const testClient: components["schemas"]["ClientDto"] = Object.freeze({
  id: "1",
  name: "Meryovi",
  lastName: "De Dios",
  email: "my@email.com",
  birthDate: "1990-01-01",
  type: 1,
  createdOnUtc: "2021-01-01T00:00:00Z",
  updatedOnUtc: null,
});

export const testAccounts: components["schemas"]["AccountDto"][] = [
  {
    id: "1",
    clientId: "1",
    type: 200,
    name: "Account 1",
    balance: 200,
    isClosed: false,
    createdOnUtc: "2021-01-01T00:00:00Z",
    closedOnUtc: null,
  },
  {
    id: "2",
    clientId: "1",
    type: 100,
    name: "Account 2",
    balance: 500,
    isClosed: false,
    createdOnUtc: "2022-01-01T00:00:00Z",
    closedOnUtc: null,
  },
  {
    id: "3",
    clientId: "1",
    type: 200,
    name: "Account 3",
    balance: 0,
    isClosed: true,
    createdOnUtc: "2021-01-01T00:00:00Z",
    closedOnUtc: "2022-01-01T00:00:00Z",
  },
];
