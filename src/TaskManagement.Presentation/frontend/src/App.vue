<script setup>
import { computed, onMounted, reactive, ref } from 'vue'

const API_URL = import.meta.env.VITE_API_URL || ''

const auth = reactive({
  token: localStorage.getItem('token') || '',
  user: JSON.parse(localStorage.getItem('user') || 'null')
})

const loginForm = reactive({ username: 'admin', password: 'admin123' })
const registerForm = reactive({ username: '', email: '', password: '' })

const taskForm = reactive({ id: 0, title: '', description: '', dueDate: '', status: 0 })
const tasks = ref([])
const loading = ref(false)
const error = ref('')
const info = ref('')
const isEditing = ref(false)

const statuses = [
  { value: 0, label: 'Pending' },
  { value: 1, label: 'In Progress' },
  { value: 2, label: 'Completed' }
]

const isAuthenticated = computed(() => Boolean(auth.token))

function setSession(token, user) {
  auth.token = token
  auth.user = user
  localStorage.setItem('token', token)
  localStorage.setItem('user', JSON.stringify(user))
}

function clearSession() {
  auth.token = ''
  auth.user = null
  localStorage.removeItem('token')
  localStorage.removeItem('user')
  tasks.value = []
}

async function request(path, options = {}) {
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {})
  }

  if (auth.token) {
    headers.Authorization = `Bearer ${auth.token}`
  }

  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers
  })

  if (!response.ok) {
    const text = await response.text()
    throw new Error(text || `Error ${response.status}`)
  }

  if (response.status === 204) {
    return null
  }

  return response.json()
}

async function login() {
  error.value = ''
  info.value = ''
  try {
    loading.value = true
    const data = await request('/api/users/login', {
      method: 'POST',
      body: JSON.stringify(loginForm)
    })
    setSession(data.token, data.user)
    info.value = `Welcome, ${data.user.username}`
    await loadTasks()
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

async function register() {
  error.value = ''
  info.value = ''
  try {
    loading.value = true
    await request('/api/users/register', {
      method: 'POST',
      body: JSON.stringify(registerForm)
    })
    info.value = 'User created. You can now login.'
    registerForm.username = ''
    registerForm.email = ''
    registerForm.password = ''
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

async function loadTasks() {
  if (!auth.token) return

  error.value = ''
  try {
    loading.value = true
    tasks.value = await request('/api/tasks')
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

function startCreate() {
  isEditing.value = false
  taskForm.id = 0
  taskForm.title = ''
  taskForm.description = ''
  taskForm.status = 0
  taskForm.dueDate = new Date(Date.now() + 86400000).toISOString().slice(0, 10)
}

function startEdit(task) {
  isEditing.value = true
  taskForm.id = task.id
  taskForm.title = task.title
  taskForm.description = task.description
  taskForm.status = task.status
  taskForm.dueDate = task.dueDate.slice(0, 10)
}

async function saveTask() {
  error.value = ''
  info.value = ''

  const payload = {
    id: taskForm.id,
    title: taskForm.title,
    description: taskForm.description,
    status: Number(taskForm.status),
    dueDate: `${taskForm.dueDate}T00:00:00`
  }

  try {
    loading.value = true
    if (isEditing.value) {
      await request(`/api/tasks/${taskForm.id}`, {
        method: 'PUT',
        body: JSON.stringify(payload)
      })
      info.value = 'Task updated.'
    } else {
      await request('/api/tasks', {
        method: 'POST',
        body: JSON.stringify(payload)
      })
      info.value = 'Task created.'
    }

    await loadTasks()
    startCreate()
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

async function removeTask(id) {
  error.value = ''
  info.value = ''
  try {
    loading.value = true
    await request(`/api/tasks/${id}`, { method: 'DELETE' })
    info.value = 'Task deleted.'
    await loadTasks()
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

function logout() {
  clearSession()
  info.value = 'Session closed.'
}

onMounted(async () => {
  startCreate()
  if (isAuthenticated.value) {
    await loadTasks()
  }
})
</script>

<template>
  <main class="page">
    <section class="hero">
      <h1>Task Command Center</h1>
      <p>Vue 3 + .NET 10 + SQLite. Login, manage tasks, stay focused.</p>
    </section>

    <p v-if="error" class="feedback error">{{ error }}</p>
    <p v-if="info" class="feedback info">{{ info }}</p>

    <section v-if="!isAuthenticated" class="grid two-col">
      <article class="card">
        <h2>Login</h2>
        <form class="stack" @submit.prevent="login">
          <input v-model="loginForm.username" placeholder="Username" required />
          <input v-model="loginForm.password" type="password" placeholder="Password" required />
          <button :disabled="loading" type="submit">Sign In</button>
        </form>
        <small>Demo user: admin / admin123</small>
      </article>

      <article class="card">
        <h2>Register</h2>
        <form class="stack" @submit.prevent="register">
          <input v-model="registerForm.username" placeholder="Username" required />
          <input v-model="registerForm.email" type="email" placeholder="Email" required />
          <input v-model="registerForm.password" type="password" placeholder="Password" required />
          <button :disabled="loading" type="submit">Create Account</button>
        </form>
      </article>
    </section>

    <section v-else class="grid app-grid">
      <article class="card">
        <div class="title-row">
          <h2>{{ isEditing ? 'Edit Task' : 'Create Task' }}</h2>
          <button class="ghost" @click="logout">Logout</button>
        </div>
        <form class="stack" @submit.prevent="saveTask">
          <input v-model="taskForm.title" placeholder="Title" required />
          <textarea v-model="taskForm.description" placeholder="Description" rows="4" required />
          <label>
            Status
            <select v-model="taskForm.status">
              <option v-for="status in statuses" :key="status.value" :value="status.value">
                {{ status.label }}
              </option>
            </select>
          </label>
          <label>
            Due date
            <input v-model="taskForm.dueDate" type="date" required />
          </label>
          <div class="actions">
            <button :disabled="loading" type="submit">{{ isEditing ? 'Update' : 'Create' }}</button>
            <button class="ghost" type="button" @click="startCreate">Reset</button>
          </div>
        </form>
      </article>

      <article class="card">
        <div class="title-row">
          <h2>Your Tasks</h2>
          <button class="ghost" :disabled="loading" @click="loadTasks">Refresh</button>
        </div>
        <ul class="task-list">
          <li v-for="task in tasks" :key="task.id" class="task-item">
            <div>
              <h3>{{ task.title }}</h3>
              <p>{{ task.description }}</p>
              <small>
                {{ statuses.find((s) => s.value === task.status)?.label }} · Due {{ task.dueDate.slice(0, 10) }}
              </small>
            </div>
            <div class="actions">
              <button class="ghost" @click="startEdit(task)">Edit</button>
              <button class="danger" @click="removeTask(task.id)">Delete</button>
            </div>
          </li>
          <li v-if="tasks.length === 0" class="empty">No tasks yet.</li>
        </ul>
      </article>
    </section>
  </main>
</template>
